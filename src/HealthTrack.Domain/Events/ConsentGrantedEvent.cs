namespace HealthTrack.Domain.Events;

public sealed class ConsentGrantedEvent : DomainEvent
{
    public Guid ConsentId { get; }
    public Guid PatientId { get; }
    public string ConsentType { get; }

    public ConsentGrantedEvent(Guid consentId, Guid patientId, string consentType)
    {
        ConsentId = consentId;
        PatientId = patientId;
        ConsentType = consentType;
    }
}
