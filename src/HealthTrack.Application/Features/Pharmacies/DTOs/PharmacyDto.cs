using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Application.Features.Pharmacies.DTOs;

public record PharmacyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Address? Address { get; init; }
    public string Phone { get; init; } = string.Empty;
    public string? Fax { get; init; }
    public bool IsActive { get; init; }
}
