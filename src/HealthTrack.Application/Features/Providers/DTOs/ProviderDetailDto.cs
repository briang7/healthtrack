namespace HealthTrack.Application.Features.Providers.DTOs;

public record ProviderDetailDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Specialty { get; init; } = string.Empty;
    public bool IsAcceptingPatients { get; init; }
    public string LicenseNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public List<AppointmentSlotDto> AppointmentSlots { get; init; } = [];
}

public record AppointmentSlotDto
{
    public Guid Id { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public bool IsAvailable { get; init; }
}
