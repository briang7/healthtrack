using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role = "Patient") : IRequest<ApiResponse<AuthResponse>>;
