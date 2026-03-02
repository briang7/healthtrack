using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class WaitlistEntry : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime RequestedDate { get; set; }
    public WaitlistPriority Priority { get; set; }
    public string Status { get; set; } = "Pending";

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
}
