using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.DeletePatient;

public sealed class DeletePatientCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePatientCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeletePatientCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        await unitOfWork.Patients.DeleteAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Patient deleted successfully.");
    }
}
