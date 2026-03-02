using FluentAssertions;
using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Tests.ValueObjects;

public class DateOfBirthTests
{
    [Fact]
    public void Constructor_WithValidPastDate_ShouldCreateSuccessfully()
    {
        // Arrange
        var date = new DateTime(1990, 6, 15, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var dob = new DateOfBirth(date);

        // Assert
        dob.Value.Should().Be(date);
    }

    [Fact]
    public void Constructor_WithFutureDate_ShouldThrowArgumentException()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act
        var act = () => new DateOfBirth(futureDate);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Date of birth must be in the past*");
    }

    [Fact]
    public void GetAge_ShouldReturnCorrectAge()
    {
        // Arrange
        var today = DateTime.UtcNow;
        var birthDate = new DateTime(today.Year - 30, today.Month, today.Day, 0, 0, 0, DateTimeKind.Utc);

        // If the birthday hasn't occurred yet this year (edge case with leap year, etc),
        // push it back a day to ensure it's in the past
        if (birthDate >= DateTime.UtcNow)
        {
            birthDate = birthDate.AddDays(-1);
        }

        var dob = new DateOfBirth(birthDate);

        // Act
        var age = dob.GetAge();

        // Assert
        age.Should().Be(30);
    }

    [Fact]
    public void GetAge_WhenBirthdayHasNotOccurredThisYear_ShouldReturnOneLessYear()
    {
        // Arrange - pick a date that is definitely in the future this year
        var today = DateTime.UtcNow;
        // Use a birthday that is "6 months ahead" from now, but 25 years ago
        var futureMonthThisYear = today.AddMonths(6);
        var birthDate = new DateTime(
            today.Year - 25,
            futureMonthThisYear.Month,
            Math.Min(futureMonthThisYear.Day, DateTime.DaysInMonth(today.Year - 25, futureMonthThisYear.Month)),
            0, 0, 0, DateTimeKind.Utc);

        var dob = new DateOfBirth(birthDate);

        // Act
        var age = dob.GetAge();

        // Assert
        age.Should().Be(24);
    }

    [Fact]
    public void TwoDateOfBirths_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var date = new DateTime(1985, 3, 20, 0, 0, 0, DateTimeKind.Utc);
        var dob1 = new DateOfBirth(date);
        var dob2 = new DateOfBirth(date);

        // Act & Assert
        dob1.Should().Be(dob2);
        (dob1 == dob2).Should().BeTrue();
        dob1.GetHashCode().Should().Be(dob2.GetHashCode());
    }

    [Fact]
    public void TwoDateOfBirths_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var dob1 = new DateOfBirth(new DateTime(1985, 3, 20, 0, 0, 0, DateTimeKind.Utc));
        var dob2 = new DateOfBirth(new DateTime(1990, 7, 10, 0, 0, 0, DateTimeKind.Utc));

        // Act & Assert
        dob1.Should().NotBe(dob2);
        (dob1 == dob2).Should().BeFalse();
    }
}
