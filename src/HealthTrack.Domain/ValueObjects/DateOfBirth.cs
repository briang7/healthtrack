namespace HealthTrack.Domain.ValueObjects;

public record DateOfBirth
{
    public DateTime Value { get; init; }

    public DateOfBirth(DateTime value)
    {
        if (value >= DateTime.UtcNow)
            throw new ArgumentException("Date of birth must be in the past.", nameof(value));
        Value = value;
    }

    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - Value.Year;
        if (Value.Date > today.AddYears(-age)) age--;
        return age;
    }
}
