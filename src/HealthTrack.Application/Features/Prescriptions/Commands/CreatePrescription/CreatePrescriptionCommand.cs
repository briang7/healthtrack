using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Attributes;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.CreatePrescription;

public record CreatePrescriptionCommand(
    Guid PatientId,
    Guid ProviderId,
    Guid? PharmacyId,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string MedicationName,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string Dosage,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string Frequency,
    DateTime StartDate,
    DateTime? EndDate,
    int RefillsRemaining,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string? Notes) : IRequest<ApiResponse<PrescriptionDto>>;
