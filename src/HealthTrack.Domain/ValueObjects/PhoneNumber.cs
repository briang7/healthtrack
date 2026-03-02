namespace HealthTrack.Domain.ValueObjects;

public record PhoneNumber
{
    public string Number { get; init; }

    public PhoneNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty.", nameof(number));
        Number = number;
    }
}
