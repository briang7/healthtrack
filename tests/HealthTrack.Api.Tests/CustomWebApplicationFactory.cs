using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Infrastructure.Identity;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace HealthTrack.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ---------------------------------------------------------------
            // 1. Replace the database: swap Npgsql for EF InMemory provider.
            //    We must remove the existing DbContextOptions<ApplicationDbContext>
            //    AND every Npgsql-specific service so EF doesn't see two providers.
            // ---------------------------------------------------------------
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                    || d.ServiceType.FullName?.StartsWith("Npgsql") == true
                    || (d.ImplementationType?.FullName?.StartsWith("Npgsql") == true)
                    || d.ServiceType.FullName?.Contains("Npgsql") == true
                    || (d.ImplementationType?.FullName?.Contains("Npgsql") == true))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            // Also remove Entity Framework relational services that were registered by Npgsql
            var relationalDescriptors = services
                .Where(d =>
                    (d.ServiceType.Assembly.FullName?.Contains("Npgsql") == true)
                    || (d.ImplementationType?.Assembly.FullName?.Contains("Npgsql") == true))
                .ToList();

            foreach (var descriptor in relationalDescriptors)
                services.Remove(descriptor);

            // Register EF Core InMemory database options for ApplicationDbContext
            var dbName = $"HealthTrackTestDb_{Guid.NewGuid()}";
            services.AddSingleton<DbContextOptions<ApplicationDbContext>>(sp =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseInMemoryDatabase(dbName);
                return optionsBuilder.Options;
            });

            // ---------------------------------------------------------------
            // 2. Replace Redis cache with in-memory distributed cache
            // ---------------------------------------------------------------
            services.RemoveAll<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            // Remove StackExchange Redis-specific registrations
            var redisDescriptors = services
                .Where(d =>
                    d.ServiceType.FullName?.Contains("StackExchange") == true
                    || d.ImplementationType?.FullName?.Contains("StackExchange") == true
                    || d.ServiceType.FullName?.Contains("Redis") == true
                    || d.ImplementationType?.FullName?.Contains("Redis") == true)
                .ToList();
            foreach (var descriptor in redisDescriptors)
                services.Remove(descriptor);

            services.RemoveAll<ICacheService>();
            services.AddDistributedMemoryCache();
            services.AddScoped<ICacheService, HealthTrack.Infrastructure.Caching.RedisCacheService>();

            // ---------------------------------------------------------------
            // 3. Initialize the database with schema and seed data
            // ---------------------------------------------------------------
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            SeedTestData(scope.ServiceProvider).GetAwaiter().GetResult();
        });
    }

    private static async Task SeedTestData(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();

        // Create roles
        string[] roles = ["Admin", "Provider", "Patient"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role));
        }

        // Create a test admin user
        if (await userManager.FindByEmailAsync("test@healthtrack.dev") is null)
        {
            var user = new ApplicationUser
            {
                UserName = "test@healthtrack.dev",
                Email = "test@healthtrack.dev",
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "User",
                Role = "Admin"
            };

            var result = await userManager.CreateAsync(user, "Test123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    /// <summary>
    /// Creates an HttpClient with a valid JWT Bearer token for the specified role.
    /// </summary>
    public HttpClient CreateAuthenticatedClient(
        string userId = "test-user-id",
        string email = "test@healthtrack.dev",
        string role = "Admin")
    {
        var client = CreateClient();

        var token = GenerateTestJwtToken(userId, email, role);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    private static string GenerateTestJwtToken(string userId, string email, string role)
    {
        const string secret = "HealthTrackSuperSecretKeyThatIsAtLeast32CharactersLong!2024";
        const string issuer = "HealthTrack";
        const string audience = "HealthTrackUsers";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
