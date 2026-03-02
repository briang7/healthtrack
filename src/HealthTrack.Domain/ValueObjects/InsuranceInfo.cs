namespace HealthTrack.Domain.ValueObjects;

public record InsuranceInfo(
    string Provider,
    string PolicyNumber,
    string GroupNumber,
    DateTime? ExpirationDate = null);
