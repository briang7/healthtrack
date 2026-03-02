using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Queries.GetProviders;

public sealed class GetProvidersQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetProvidersQuery, ApiResponse<PagedResult<ProviderDto>>>
{
    public async Task<ApiResponse<PagedResult<ProviderDto>>> Handle(
        GetProvidersQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Provider> providers;

        if (!string.IsNullOrWhiteSpace(request.Specialty))
        {
            providers = await unitOfWork.Providers
                .GetBySpecialtyAsync(request.Specialty, cancellationToken);
        }
        else if (request.AcceptingPatients == true)
        {
            providers = await unitOfWork.Providers
                .GetAcceptingPatientsAsync(cancellationToken);
        }
        else
        {
            providers = await unitOfWork.Providers.GetAllAsync(cancellationToken);
        }

        var filtered = providers.AsEnumerable();

        if (request.AcceptingPatients.HasValue && !string.IsNullOrWhiteSpace(request.Specialty))
            filtered = filtered.Where(p => p.IsAcceptingPatients == request.AcceptingPatients.Value);

        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;

        var paged = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<ProviderDto>>(paged);

        var result = new PagedResult<ProviderDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<ProviderDto>>.SuccessResponse(result);
    }
}
