using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Queries.GetProviders;

public record GetProvidersQuery(
    string? Specialty = null,
    bool? AcceptingPatients = null,
    int Page = 1,
    int PageSize = 10) : IRequest<ApiResponse<PagedResult<ProviderDto>>>;
