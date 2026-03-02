namespace HealthTrack.Domain.Events;

public sealed class AppointmentCancelledEvent : DomainEvent
{
    public Guid AppointmentId { get; }
    public string Reason { get; }

    public AppointmentCancelledEvent(Guid appointmentId, string reason)
    {
        AppointmentId = appointmentId;
        Reason = reason;
    }
}
