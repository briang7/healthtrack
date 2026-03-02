using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<ApiResponse<AuthResponse>>;
