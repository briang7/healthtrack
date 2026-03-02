using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Entities;

public class Pharmacy : BaseEntity
{
    public required string Name { get; set; }
    public required Address Address { get; set; }
    public required PhoneNumber Phone { get; set; }
    public string? Fax { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<Prescription> Prescriptions { get; set; } = [];
}
