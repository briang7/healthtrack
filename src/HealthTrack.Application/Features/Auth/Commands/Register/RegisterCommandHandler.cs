using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, ApiResponse<AuthResponse>>
{
    public async Task<ApiResponse<AuthResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var (success, userId, errors) = await identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Role);

        if (!success)
            return ApiResponse<AuthResponse>.FailureResponse(errors.ToList());

        var authResponse = await identityService.GenerateTokensAsync(
            userId,
            request.Email,
            request.Role);

        return authResponse is null
            ? ApiResponse<AuthResponse>.FailureResponse("Failed to generate authentication tokens.")
            : ApiResponse<AuthResponse>.SuccessResponse(authResponse, "Registration successful.");
    }
}
