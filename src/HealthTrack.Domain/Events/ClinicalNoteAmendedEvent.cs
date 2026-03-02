namespace HealthTrack.Domain.Events;

public sealed class ClinicalNoteAmendedEvent : DomainEvent
{
    public Guid NoteId { get; }
    public Guid PreviousVersionId { get; }
    public Guid ProviderId { get; }

    public ClinicalNoteAmendedEvent(Guid noteId, Guid previousVersionId, Guid providerId)
    {
        NoteId = noteId;
        PreviousVersionId = previousVersionId;
        ProviderId = providerId;
    }
}
