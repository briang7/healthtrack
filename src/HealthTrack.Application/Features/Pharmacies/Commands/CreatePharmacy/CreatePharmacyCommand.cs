using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.CreatePharmacy;

public record CreatePharmacyCommand(
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string Phone,
    string? Fax) : IRequest<ApiResponse<PharmacyDto>>;
