using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Entities;

public class Provider : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Specialty { get; set; }
    public required string LicenseNumber { get; set; }
    public required string Email { get; set; }
    public required PhoneNumber Phone { get; set; }
    public bool IsAcceptingPatients { get; set; } = true;

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Prescription> Prescriptions { get; set; } = [];
    public ICollection<ClinicalNote> ClinicalNotes { get; set; } = [];
    public ICollection<AppointmentSlot> AppointmentSlots { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}";
}
