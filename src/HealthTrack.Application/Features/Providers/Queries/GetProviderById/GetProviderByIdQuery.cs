using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Queries.GetProviderById;

public record GetProviderByIdQuery(Guid Id) : IRequest<ApiResponse<ProviderDetailDto>>;
