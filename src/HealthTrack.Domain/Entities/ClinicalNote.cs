using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Entities;

public class ClinicalNote : BaseEntity
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? AppointmentId { get; set; }
    public NoteType NoteType { get; set; }
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
    public int Version { get; set; } = 1;
    public bool IsAmended { get; set; }
    public Guid? PreviousVersionId { get; set; }

    // Navigation properties
    public Patient Patient { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public ClinicalNote? PreviousVersion { get; set; }
}
