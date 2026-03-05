using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class Prescription : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? PharmacyId { get; set; }

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public required string MedicationName { get; set; }

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public required string Dosage { get; set; }

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public required string Frequency { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int RefillsRemaining { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public string? Notes { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public Pharmacy? Pharmacy { get; set; }
}
