using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Entities;

public class Patient : BaseEntity
{
    [Phi(PhiSensitivity.Standard, Category = "Demographics")]
    public required string FirstName { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Demographics")]
    public required string LastName { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Demographics")]
    public required DateOfBirth DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Contact")]
    public required string Email { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Contact")]
    public required PhoneNumber Phone { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Contact")]
    public required Address Address { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Financial")]
    public InsuranceInfo? InsuranceInfo { get; set; }

    [Phi(PhiSensitivity.Standard, Category = "Contact")]
    public EmergencyContact? EmergencyContact { get; set; }

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public string? MedicalHistory { get; set; }

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public List<string> Allergies { get; set; } = [];

    [Phi(PhiSensitivity.Sensitive, Category = "Clinical")]
    public List<string> Medications { get; set; } = [];

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = [];
    public ICollection<Consent> Consents { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
