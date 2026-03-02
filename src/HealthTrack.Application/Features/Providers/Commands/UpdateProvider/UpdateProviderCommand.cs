using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Commands.UpdateProvider;

public record UpdateProviderCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialty,
    string LicenseNumber,
    string Email,
    string Phone,
    bool IsAcceptingPatients) : IRequest<ApiResponse<ProviderDto>>;
