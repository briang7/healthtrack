namespace HealthTrack.Domain.Entities;

public class AppointmentSlot : BaseEntity
{
    public Guid ProviderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    public Provider Provider { get; set; } = null!;
}
