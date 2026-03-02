using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.RequestRefill;

public sealed class RequestRefillCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<RequestRefillCommand, ApiResponse<PrescriptionDto>>
{
    public async Task<ApiResponse<PrescriptionDto>> Handle(
        RequestRefillCommand request,
        CancellationToken cancellationToken)
    {
        var prescription = await unitOfWork.Prescriptions.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Prescription), request.Id);

        if (prescription.Status != PrescriptionStatus.Active)
            return ApiResponse<PrescriptionDto>.FailureResponse("Only active prescriptions can be refilled.");

        if (prescription.RefillsRemaining <= 0)
            return ApiResponse<PrescriptionDto>.FailureResponse("No refills remaining for this prescription.");

        prescription.RefillsRemaining--;
        prescription.Status = PrescriptionStatus.RefillRequested;
        prescription.ModifiedAt = DateTime.UtcNow;
        prescription.ModifiedBy = currentUserService.UserId;

        await unitOfWork.Prescriptions.UpdateAsync(prescription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PrescriptionDto>(prescription);
        return ApiResponse<PrescriptionDto>.SuccessResponse(dto, "Refill requested successfully.");
    }
}
