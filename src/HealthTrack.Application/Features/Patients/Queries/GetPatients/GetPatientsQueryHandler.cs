using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Queries.GetPatients;

public sealed class GetPatientsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPatientsQuery, ApiResponse<PagedResult<PatientDto>>>
{
    public async Task<ApiResponse<PagedResult<PatientDto>>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var patients = string.IsNullOrWhiteSpace(request.SearchTerm)
            ? await unitOfWork.Patients.GetAllAsync(cancellationToken)
            : await unitOfWork.Patients.SearchAsync(request.SearchTerm, cancellationToken);

        var totalCount = patients.Count;

        var pagedPatients = patients
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<PatientDto>>(pagedPatients);

        var result = new PagedResult<PatientDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<PatientDto>>.SuccessResponse(result);
    }
}
