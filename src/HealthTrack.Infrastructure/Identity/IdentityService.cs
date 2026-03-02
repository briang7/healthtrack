using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.Identity;

namespace HealthTrack.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public IdentityService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<(bool Success, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email, string password, string firstName, string lastName, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return (result.Succeeded, user.Id, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Success, string UserId, string Role)> ValidateUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return (false, string.Empty, string.Empty);

        var isValid = await _userManager.CheckPasswordAsync(user, password);

        return isValid
            ? (true, user.Id, user.Role)
            : (false, string.Empty, string.Empty);
    }

    public async Task<AuthResponse?> GenerateTokensAsync(string userId, string email, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return null;

        var token = await _tokenService.GenerateTokenAsync(userId, email, role);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponse(
            Token: token,
            RefreshToken: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddHours(1),
            UserId: userId,
            Email: email,
            Role: role);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = await _tokenService.GetPrincipalFromExpiredTokenAsync(token);
        if (principal is null) return null;

        var (userId, email, role) = principal.Value;
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null ||
            user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return await GenerateTokensAsync(userId, email, role);
    }
}
