using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    [property: System.ComponentModel.DefaultValue("patient@healthtrack.dev")] string Email,
    [property: System.ComponentModel.DefaultValue("Demo123!")] string Password
) : IRequest<ApiResponse<AuthResponse>>;
