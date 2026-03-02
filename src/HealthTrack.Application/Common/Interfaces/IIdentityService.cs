using HealthTrack.Application.Features.Auth.DTOs;

namespace HealthTrack.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Success, string UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email, string password, string firstName, string lastName, string role);

    Task<(bool Success, string UserId, string Role)> ValidateUserAsync(string email, string password);

    Task<AuthResponse?> GenerateTokensAsync(string userId, string email, string role);

    Task<AuthResponse?> RefreshTokenAsync(string token, string refreshToken);
}
