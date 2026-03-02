using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponse>>
{
    public async Task<ApiResponse<AuthResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var authResponse = await identityService.RefreshTokenAsync(
            request.Token,
            request.RefreshToken);

        return authResponse is null
            ? ApiResponse<AuthResponse>.FailureResponse("Invalid or expired refresh token.")
            : ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Token refreshed successfully.");
    }
}
