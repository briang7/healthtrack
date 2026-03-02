namespace HealthTrack.Application.Features.Auth.DTOs;

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string Role);
