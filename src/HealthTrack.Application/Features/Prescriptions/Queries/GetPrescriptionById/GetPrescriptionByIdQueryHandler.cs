using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptionById;

public sealed class GetPrescriptionByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPrescriptionByIdQuery, ApiResponse<PrescriptionDto>>
{
    public async Task<ApiResponse<PrescriptionDto>> Handle(
        GetPrescriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var prescription = await unitOfWork.Prescriptions.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Prescription), request.Id);

        var dto = mapper.Map<PrescriptionDto>(prescription);
        return ApiResponse<PrescriptionDto>.SuccessResponse(dto);
    }
}
