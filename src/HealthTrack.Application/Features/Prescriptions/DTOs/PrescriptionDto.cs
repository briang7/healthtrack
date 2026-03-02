using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Features.Prescriptions.DTOs;

public record PrescriptionDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid ProviderId { get; init; }
    public string ProviderName { get; init; } = string.Empty;
    public string MedicationName { get; init; } = string.Empty;
    public string Dosage { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int RefillsRemaining { get; init; }
    public PrescriptionStatus Status { get; init; }
    public string? PharmacyName { get; init; }
}
