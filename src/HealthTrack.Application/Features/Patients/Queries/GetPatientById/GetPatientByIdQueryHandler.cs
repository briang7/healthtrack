using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Queries.GetPatientById;

public sealed class GetPatientByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPatientByIdQuery, ApiResponse<PatientDetailDto>>
{
    public async Task<ApiResponse<PatientDetailDto>> Handle(
        GetPatientByIdQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetWithAppointmentsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        var dto = mapper.Map<PatientDetailDto>(patient);
        return ApiResponse<PatientDetailDto>.SuccessResponse(dto);
    }
}
