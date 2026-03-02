using HealthTrack.Domain.Enums;
using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Entities;

public class Patient : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateOfBirth DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public required string Email { get; set; }
    public required PhoneNumber Phone { get; set; }
    public required Address Address { get; set; }
    public InsuranceInfo? InsuranceInfo { get; set; }
    public EmergencyContact? EmergencyContact { get; set; }
    public string? MedicalHistory { get; set; }
    public List<string> Allergies { get; set; } = [];
    public List<string> Medications { get; set; } = [];
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = [];
    public ICollection<Consent> Consents { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
