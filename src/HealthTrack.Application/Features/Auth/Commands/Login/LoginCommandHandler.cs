using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, ApiResponse<AuthResponse>>
{
    public async Task<ApiResponse<AuthResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var (success, userId, role) = await identityService.ValidateUserAsync(
            request.Email,
            request.Password);

        if (!success)
            return ApiResponse<AuthResponse>.FailureResponse("Invalid email or password.");

        var authResponse = await identityService.GenerateTokensAsync(userId, request.Email, role);

        return authResponse is null
            ? ApiResponse<AuthResponse>.FailureResponse("Failed to generate authentication tokens.")
            : ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Login successful.");
    }
}
