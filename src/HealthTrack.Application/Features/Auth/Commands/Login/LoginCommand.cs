using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<ApiResponse<AuthResponse>>;
