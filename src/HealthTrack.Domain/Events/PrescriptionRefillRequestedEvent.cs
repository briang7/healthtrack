namespace HealthTrack.Domain.Events;

public sealed class PrescriptionRefillRequestedEvent : DomainEvent
{
    public Guid PrescriptionId { get; }
    public Guid PatientId { get; }

    public PrescriptionRefillRequestedEvent(Guid prescriptionId, Guid patientId)
    {
        PrescriptionId = prescriptionId;
        PatientId = patientId;
    }
}
