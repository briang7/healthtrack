using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class Consent : BaseEntity
{
    public Guid PatientId { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Legal")]
    public ConsentType ConsentType { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Legal")]
    public DateTime GrantedAt { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Legal")]
    public DateTime? RevokedAt { get; set; }

    public string? DocumentUrl { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
}
