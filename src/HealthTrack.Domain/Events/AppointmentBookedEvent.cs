namespace HealthTrack.Domain.Events;

public sealed class AppointmentBookedEvent : DomainEvent
{
    public Guid AppointmentId { get; }
    public Guid PatientId { get; }
    public Guid ProviderId { get; }
    public DateTime ScheduledAt { get; }

    public AppointmentBookedEvent(Guid appointmentId, Guid patientId, Guid providerId, DateTime scheduledAt)
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        ProviderId = providerId;
        ScheduledAt = scheduledAt;
    }
}
