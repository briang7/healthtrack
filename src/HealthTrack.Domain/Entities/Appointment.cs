using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public TimeSpan Duration { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public AppointmentType Type { get; set; }
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrencePattern { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public ClinicalNote? ClinicalNote { get; set; }
}
