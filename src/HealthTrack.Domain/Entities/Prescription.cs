using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class Prescription : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? PharmacyId { get; set; }
    public required string MedicationName { get; set; }
    public required string Dosage { get; set; }
    public required string Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int RefillsRemaining { get; set; }
    public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;
    public string? Notes { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public Pharmacy? Pharmacy { get; set; }
}
