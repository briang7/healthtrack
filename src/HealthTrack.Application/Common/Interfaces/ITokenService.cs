namespace HealthTrack.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(string userId, string email, string role);
    string GenerateRefreshToken();
    Task<(string userId, string email, string role)?> GetPrincipalFromExpiredTokenAsync(string token);
}
