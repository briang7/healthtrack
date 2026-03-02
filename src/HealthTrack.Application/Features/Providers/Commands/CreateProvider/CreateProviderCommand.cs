using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Commands.CreateProvider;

public record CreateProviderCommand(
    string FirstName,
    string LastName,
    string Specialty,
    string LicenseNumber,
    string Email,
    string Phone,
    bool IsAcceptingPatients = true) : IRequest<ApiResponse<ProviderDto>>;
