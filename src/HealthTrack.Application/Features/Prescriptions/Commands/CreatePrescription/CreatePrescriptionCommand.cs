using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.CreatePrescription;

public record CreatePrescriptionCommand(
    Guid PatientId,
    Guid ProviderId,
    Guid? PharmacyId,
    string MedicationName,
    string Dosage,
    string Frequency,
    DateTime StartDate,
    DateTime? EndDate,
    int RefillsRemaining,
    string? Notes) : IRequest<ApiResponse<PrescriptionDto>>;
