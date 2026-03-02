using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.CheckDrugInteractions;

public sealed class CheckDrugInteractionsCommandHandler(
    IUnitOfWork unitOfWork,
    IDrugInteractionService drugInteractionService)
    : IRequestHandler<CheckDrugInteractionsCommand, ApiResponse<List<DrugInteraction>>>
{
    public async Task<ApiResponse<List<DrugInteraction>>> Handle(
        CheckDrugInteractionsCommand request,
        CancellationToken cancellationToken)
    {
        _ = await unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var activePrescriptions = await unitOfWork.Prescriptions
            .GetActivePrescriptionsAsync(request.PatientId, cancellationToken);

        var currentMedications = activePrescriptions
            .Where(p => p.Status == PrescriptionStatus.Active)
            .Select(p => p.MedicationName)
            .ToList();

        if (currentMedications.Count == 0)
            return ApiResponse<List<DrugInteraction>>.SuccessResponse([], "No active medications to check against.");

        var interactions = await drugInteractionService.CheckInteractionsAsync(
            request.Medication,
            currentMedications,
            cancellationToken);

        var message = interactions.Count > 0
            ? $"Found {interactions.Count} potential drug interaction(s)."
            : "No drug interactions found.";

        return ApiResponse<List<DrugInteraction>>.SuccessResponse(interactions, message);
    }
}
