using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Features.ClinicalNotes.DTOs;

public record ClinicalNoteDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid ProviderId { get; init; }
    public string ProviderName { get; init; } = string.Empty;
    public NoteType NoteType { get; init; }
    public string? Subjective { get; init; }
    public string? Objective { get; init; }
    public string? Assessment { get; init; }
    public string? Plan { get; init; }
    public int Version { get; init; }
    public bool IsAmended { get; init; }
    public Guid? AppointmentId { get; init; }
    public DateTime CreatedAt { get; init; }
}
