using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptions;

public sealed class GetPrescriptionsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPrescriptionsQuery, ApiResponse<PagedResult<PrescriptionDto>>>
{
    public async Task<ApiResponse<PagedResult<PrescriptionDto>>> Handle(
        GetPrescriptionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Prescription> prescriptions;

        if (request.PatientId.HasValue)
        {
            prescriptions = await unitOfWork.Prescriptions
                .GetByPatientIdAsync(request.PatientId.Value, cancellationToken);
        }
        else
        {
            prescriptions = await unitOfWork.Prescriptions.GetAllAsync(cancellationToken);
        }

        var filtered = prescriptions.AsEnumerable();

        if (request.Status.HasValue)
            filtered = filtered.Where(p => p.Status == request.Status.Value);

        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;

        var paged = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<PrescriptionDto>>(paged);

        var result = new PagedResult<PrescriptionDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<PrescriptionDto>>.SuccessResponse(result);
    }
}
