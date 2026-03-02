using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.UpdatePharmacy;

public record UpdatePharmacyCommand(
    Guid Id,
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string Phone,
    string? Fax,
    bool IsActive) : IRequest<ApiResponse<PharmacyDto>>;
