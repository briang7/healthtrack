using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class Consent : BaseEntity
{
    public Guid PatientId { get; set; }
    public ConsentType ConsentType { get; set; }
    public DateTime GrantedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? DocumentUrl { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
}
