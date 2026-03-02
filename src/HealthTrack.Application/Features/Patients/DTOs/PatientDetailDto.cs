using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Application.Features.Patients.DTOs;

public record PatientDetailDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public DateTime DateOfBirth { get; init; }
    public bool IsActive { get; init; }
    public Address? Address { get; init; }
    public InsuranceInfo? InsuranceInfo { get; init; }
    public EmergencyContact? EmergencyContact { get; init; }
    public string? MedicalHistory { get; init; }
    public List<string> Allergies { get; init; } = [];
    public List<string> Medications { get; init; } = [];
    public List<AppointmentDto> Appointments { get; init; } = [];
    public List<PrescriptionDto> Prescriptions { get; init; } = [];
}
