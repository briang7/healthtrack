using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Features.Patients.DTOs;

public record PatientDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public DateTime DateOfBirth { get; init; }
    public bool IsActive { get; init; }
}
