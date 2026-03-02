using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Queries.GetPharmacies;

public sealed class GetPharmaciesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPharmaciesQuery, ApiResponse<PagedResult<PharmacyDto>>>
{
    public async Task<ApiResponse<PagedResult<PharmacyDto>>> Handle(
        GetPharmaciesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Pharmacy> pharmacies;

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            pharmacies = await unitOfWork.Pharmacies.SearchByNameAsync(request.SearchTerm, cancellationToken);
        }
        else
        {
            pharmacies = await unitOfWork.Pharmacies.GetAllAsync(cancellationToken);
        }

        var totalCount = pharmacies.Count;

        var paged = pharmacies
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<PharmacyDto>>(paged);

        var result = new PagedResult<PharmacyDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<PharmacyDto>>.SuccessResponse(result);
    }
}
