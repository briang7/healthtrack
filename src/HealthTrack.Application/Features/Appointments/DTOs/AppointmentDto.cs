using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Features.Appointments.DTOs;

public record AppointmentDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public Guid ProviderId { get; init; }
    public string ProviderName { get; init; } = string.Empty;
    public DateTime ScheduledAt { get; init; }
    public TimeSpan Duration { get; init; }
    public AppointmentStatus Status { get; init; }
    public AppointmentType Type { get; init; }
    public string? Notes { get; init; }
}
