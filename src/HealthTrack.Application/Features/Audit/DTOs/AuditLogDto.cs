namespace HealthTrack.Application.Features.Audit.DTOs;

public record AuditLogDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public string? IpAddress { get; init; }
    public DateTime Timestamp { get; init; }
}
