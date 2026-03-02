using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Queries.GetPharmacies;

public record GetPharmaciesQuery(
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10) : IRequest<ApiResponse<PagedResult<PharmacyDto>>>;
