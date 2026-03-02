using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Queries.GetPharmacyById;

public record GetPharmacyByIdQuery(Guid Id) : IRequest<ApiResponse<PharmacyDto>>;
