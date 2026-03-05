using HealthTrack.Api.Middleware;
using HealthTrack.Api.Services;
using HealthTrack.Application;
using HealthTrack.Infrastructure;
using HealthTrack.Infrastructure.Identity;
using HealthTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Current user service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HealthTrack.Application.Common.Interfaces.ICurrentUserService, CurrentUserService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Authorization policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("ProviderOrAdmin", policy => policy.RequireRole("Provider", "Admin"))
    .AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HealthTrack API",
        Version = "v1",
        Description = "HIPAA-aware Patient Portal REST API built with ASP.NET Core 9, Clean Architecture, CQRS (MediatR), JWT Auth, PostgreSQL, and Redis."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token (no 'Bearer' prefix needed)",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Health checks
var healthChecks = builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database");
var redisConn = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConn))
    healthChecks.AddRedis(redisConn, name: "redis");

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger + landing page enabled in all environments (portfolio demo)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthTrack API v1");
        c.DocumentTitle = "HealthTrack API - Patient Portal";
        // Auto-apply JWT token if passed via query string from landing page
        c.HeadContent = """
            <style>
                .ht-bar { background: #1e293b; border-bottom: 1px solid #334155; padding: 8px 16px; display: flex; align-items: center; justify-content: space-between; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif; font-size: 13px; position: sticky; top: 0; z-index: 9999; }
                .ht-bar a { color: #60a5fa; text-decoration: none; }
                .ht-bar a:hover { text-decoration: underline; }
                .ht-status { color: #94a3b8; }
                .ht-status .active { color: #34d399; font-weight: 600; }
                .ht-status .none { color: #f87171; }
                /* Hide Swagger/SmartBear branding */
                .swagger-ui .topbar { display: none !important; }
            </style>
            <script>
            (function() {
                function applyToken(token) {
                    const interval = setInterval(() => {
                        if (window.ui) {
                            clearInterval(interval);
                            // Use authActions.authorize - the reliable way to set auth
                            window.ui.authActions.authorize({
                                Bearer: {
                                    name: 'Bearer',
                                    schema: { type: 'http', scheme: 'bearer', bearerFormat: 'JWT' },
                                    value: token
                                }
                            });
                            updateBar(token);
                        }
                    }, 300);
                }
                function clearAuth() {
                    if (window.ui) {
                        window.ui.authActions.logout(['Bearer']);
                    }
                }
                function parseJwt(token) {
                    try { return JSON.parse(atob(token.split('.')[1])); } catch { return null; }
                }
                function updateBar(token) {
                    const bar = document.getElementById('ht-bar');
                    if (!bar) return;
                    const statusEl = bar.querySelector('.ht-status');
                    if (token) {
                        const claims = parseJwt(token);
                        const role = claims?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Unknown';
                        const email = claims?.email || '';
                        statusEl.innerHTML = 'Signed in as <span class="active">' + email + '</span> (' + role + ') &mdash; <a href="/" id="ht-switch">switch role</a> &middot; <a href="javascript:void(0)" id="ht-logout">sign out</a>';
                        document.getElementById('ht-logout').onclick = function() {
                            localStorage.removeItem('healthtrack_token');
                            clearAuth();
                            updateBar(null);
                        };
                    } else {
                        statusEl.innerHTML = '<span class="none">Not authenticated</span> &mdash; <a href="/">sign in with a demo account</a>';
                    }
                }
                window.addEventListener('DOMContentLoaded', function() {
                    // Inject bar
                    const bar = document.createElement('div');
                    bar.className = 'ht-bar';
                    bar.id = 'ht-bar';
                    bar.innerHTML = '<a href="/" style="font-weight:600;color:#e2e8f0;">HealthTrack API</a><span class="ht-status">Loading...</span>';
                    document.body.prepend(bar);

                    // Check for token from landing page redirect
                    const params = new URLSearchParams(window.location.search);
                    const urlToken = params.get('token');
                    if (urlToken) {
                        localStorage.setItem('healthtrack_token', urlToken);
                        history.replaceState(null, '', '/swagger/index.html');
                        applyToken(urlToken);
                    } else {
                        // Restore from localStorage
                        const saved = localStorage.getItem('healthtrack_token');
                        if (saved) {
                            // Check expiry
                            const claims = parseJwt(saved);
                            if (claims?.exp && claims.exp * 1000 > Date.now()) {
                                applyToken(saved);
                            } else {
                                localStorage.removeItem('healthtrack_token');
                                updateBar(null);
                            }
                        } else {
                            setTimeout(() => updateBar(null), 500);
                        }
                    }
                });
            })();
            </script>
            """;
    });

    app.UseStaticFiles();

    // Landing page with one-click demo login
    app.MapGet("/", () => Results.Content("""
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>HealthTrack API - Patient Portal</title>
        <style>
            * { margin: 0; padding: 0; box-sizing: border-box; }
            body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; background: #0f172a; color: #e2e8f0; min-height: 100vh; display: flex; align-items: center; justify-content: center; }
            .container { max-width: 720px; width: 100%; padding: 2rem; }
            .badge { display: inline-block; background: #1e40af; color: #93c5fd; font-size: 0.75rem; font-weight: 600; padding: 0.25rem 0.75rem; border-radius: 9999px; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 1rem; }
            h1 { font-size: 2.5rem; font-weight: 700; margin-bottom: 0.5rem; background: linear-gradient(135deg, #60a5fa, #34d399); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
            .subtitle { color: #94a3b8; font-size: 1.1rem; margin-bottom: 2.5rem; line-height: 1.6; }
            .tech { display: flex; flex-wrap: wrap; gap: 0.5rem; margin-bottom: 2.5rem; }
            .tech span { background: #1e293b; border: 1px solid #334155; padding: 0.3rem 0.75rem; border-radius: 6px; font-size: 0.8rem; color: #cbd5e1; }
            h2 { font-size: 1rem; text-transform: uppercase; letter-spacing: 0.1em; color: #64748b; margin-bottom: 1rem; }
            .roles { display: flex; flex-direction: column; gap: 0.75rem; margin-bottom: 2rem; }
            .role-btn { display: flex; align-items: center; gap: 1rem; background: #1e293b; border: 1px solid #334155; border-radius: 12px; padding: 1rem 1.25rem; cursor: pointer; transition: all 0.2s; text-decoration: none; color: inherit; }
            .role-btn:hover { border-color: #60a5fa; background: #1e3a5f; transform: translateY(-1px); }
            .role-btn.loading { opacity: 0.7; pointer-events: none; }
            .role-icon { width: 48px; height: 48px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; flex-shrink: 0; }
            .role-icon.admin { background: #7c3aed20; color: #a78bfa; }
            .role-icon.provider { background: #05966920; color: #34d399; }
            .role-icon.patient { background: #2563eb20; color: #60a5fa; }
            .role-info { flex: 1; }
            .role-name { font-weight: 600; font-size: 1.05rem; margin-bottom: 0.2rem; }
            .role-desc { color: #64748b; font-size: 0.85rem; }
            .role-arrow { color: #475569; font-size: 1.2rem; transition: transform 0.2s; }
            .role-btn:hover .role-arrow { transform: translateX(4px); color: #60a5fa; }
            .swagger-link { display: inline-flex; align-items: center; gap: 0.5rem; color: #64748b; font-size: 0.9rem; text-decoration: none; margin-top: 1rem; transition: color 0.2s; }
            .swagger-link:hover { color: #60a5fa; }
            .data-info { background: #1e293b; border: 1px solid #334155; border-radius: 12px; padding: 1rem 1.25rem; margin-bottom: 2rem; }
            .data-info p { color: #94a3b8; font-size: 0.85rem; line-height: 1.6; }
            .data-info strong { color: #e2e8f0; }
            .error-msg { color: #f87171; font-size: 0.85rem; margin-top: 0.5rem; display: none; }
            .spinner { display: none; width: 18px; height: 18px; border: 2px solid #475569; border-top-color: #60a5fa; border-radius: 50%; animation: spin 0.6s linear infinite; }
            @keyframes spin { to { transform: rotate(360deg); } }
            .cs-overlay { position: fixed; inset: 0; background: #0f172a; z-index: 10000; display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 1.5rem; }
            .cs-spinner { width: 40px; height: 40px; border: 3px solid #334155; border-top-color: #60a5fa; border-radius: 50%; animation: spin 0.8s linear infinite; }
            .cs-text { color: #94a3b8; font-size: 1rem; }
        </style>
    </head>
    <body>
        <div class="cs-overlay" id="cold-start">
            <div class="cs-spinner"></div>
            <div class="cs-text">Connecting to server...</div>
        </div>
        <div class="container">
            <div class="badge">Portfolio Project</div>
            <h1>HealthTrack API</h1>
            <p class="subtitle">HIPAA-aware Patient Portal REST API built with Clean Architecture, CQRS, and enterprise .NET patterns.</p>

            <div class="tech">
                <span>ASP.NET Core 9</span>
                <span>Clean Architecture</span>
                <span>CQRS / MediatR</span>
                <span>PostgreSQL</span>
                <span>Redis</span>
                <span>JWT Auth</span>
                <span>EF Core 9</span>
                <span>FluentValidation</span>
            </div>

            <h2>Sign in as a demo user</h2>
            <div class="roles">
                <a class="role-btn" onclick="login('admin@healthtrack.dev', this)" href="javascript:void(0)">
                    <div class="role-icon admin">&#x1f6e1;</div>
                    <div class="role-info">
                        <div class="role-name">Admin</div>
                        <div class="role-desc">Full access &mdash; manage users, view audit logs, all CRUD operations</div>
                    </div>
                    <div class="spinner"></div>
                    <div class="role-arrow">&rarr;</div>
                </a>
                <a class="role-btn" onclick="login('provider@healthtrack.dev', this)" href="javascript:void(0)">
                    <div class="role-icon provider">&#x1fa7a;</div>
                    <div class="role-info">
                        <div class="role-name">Provider</div>
                        <div class="role-desc">Doctor view &mdash; patients, appointments, prescriptions, clinical notes</div>
                    </div>
                    <div class="spinner"></div>
                    <div class="role-arrow">&rarr;</div>
                </a>
                <a class="role-btn" onclick="login('patient@healthtrack.dev', this)" href="javascript:void(0)">
                    <div class="role-icon patient">&#x1f464;</div>
                    <div class="role-info">
                        <div class="role-name">Patient</div>
                        <div class="role-desc">Patient view &mdash; own records, appointments, prescriptions</div>
                    </div>
                    <div class="spinner"></div>
                    <div class="role-arrow">&rarr;</div>
                </a>
            </div>

            <div class="data-info">
                <p>Pre-seeded with <strong>3 providers</strong>, <strong>10 patients</strong>, <strong>20 appointments</strong>, <strong>15 prescriptions</strong>, <strong>10 clinical notes</strong>, and <strong>5 pharmacies</strong>.</p>
            </div>

            <div class="error-msg" id="error"></div>

            <a class="swagger-link" href="/swagger">
                Or browse the API docs without authentication &rarr;
            </a>
        </div>

        <script>
            // Show loading overlay on first visit (cold start on Render free tier)
            (function() {
                const overlay = document.getElementById('cold-start');
                if (!overlay) return;
                fetch('/api/v1/health').then(r => {
                    if (r.ok) overlay.remove();
                    else throw new Error();
                }).catch(() => {
                    overlay.querySelector('.cs-text').textContent = 'Server is waking up... this may take 30-60 seconds';
                    const poll = setInterval(() => {
                        fetch('/api/v1/health').then(r => {
                            if (r.ok) { clearInterval(poll); overlay.remove(); }
                        }).catch(() => {});
                    }, 3000);
                });
            })();

            async function login(email, btn) {
                const spinner = btn.querySelector('.spinner');
                const arrow = btn.querySelector('.role-arrow');
                const errorEl = document.getElementById('error');
                errorEl.style.display = 'none';
                btn.classList.add('loading');
                spinner.style.display = 'block';
                arrow.style.display = 'none';
                try {
                    const res = await fetch('/api/v1/auth/login', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ email, password: 'Demo123!' })
                    });
                    const data = await res.json();
                    if (data.success && data.data?.token) {
                        // Store token and redirect to Swagger with auth pre-configured
                        const token = data.data.token;
                        // Swagger UI reads auth from localStorage
                        localStorage.setItem('healthtrack_token', token);
                        // Redirect to swagger with token in URL hash for the authorize interceptor
                        window.location.href = '/swagger/index.html?token=' + encodeURIComponent(token);
                    } else {
                        throw new Error(data.errors?.join(', ') || 'Login failed');
                    }
                } catch (e) {
                    errorEl.textContent = 'Login failed: ' + e.message;
                    errorEl.style.display = 'block';
                    btn.classList.remove('loading');
                    spinner.style.display = 'none';
                    arrow.style.display = 'block';
                }
            }
        </script>
    </body>
    </html>
    """, "text/html")).ExcludeFromDescription();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Auto-migrate and seed (all environments for portfolio demo)
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await HealthTrack.Infrastructure.Persistence.SeedData.SeedAsync(scope.ServiceProvider);
}

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
