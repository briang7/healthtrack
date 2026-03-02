namespace HealthTrack.Domain.ValueObjects;

public record EmergencyContact(
    string Name,
    string Relationship,
    string Phone);
