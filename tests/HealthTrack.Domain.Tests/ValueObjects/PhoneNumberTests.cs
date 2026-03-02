using FluentAssertions;
using HealthTrack.Domain.ValueObjects;

namespace HealthTrack.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Constructor_WithValidNumber_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var phone = new PhoneNumber("(555) 123-4567");

        // Assert
        phone.Number.Should().Be("(555) 123-4567");
    }

    [Fact]
    public void Constructor_WithSimpleDigits_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var phone = new PhoneNumber("5551234567");

        // Assert
        phone.Number.Should().Be("5551234567");
    }

    [Fact]
    public void Constructor_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new PhoneNumber(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Phone number cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNull_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new PhoneNumber(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Phone number cannot be empty*");
    }

    [Fact]
    public void Constructor_WithWhitespace_ShouldThrowArgumentException()
    {
        // Arrange & Act
        var act = () => new PhoneNumber("   ");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Phone number cannot be empty*");
    }

    [Fact]
    public void TwoPhoneNumbers_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var phone1 = new PhoneNumber("(555) 123-4567");
        var phone2 = new PhoneNumber("(555) 123-4567");

        // Act & Assert
        phone1.Should().Be(phone2);
        (phone1 == phone2).Should().BeTrue();
        phone1.GetHashCode().Should().Be(phone2.GetHashCode());
    }

    [Fact]
    public void TwoPhoneNumbers_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var phone1 = new PhoneNumber("(555) 123-4567");
        var phone2 = new PhoneNumber("(555) 987-6543");

        // Act & Assert
        phone1.Should().NotBe(phone2);
        (phone1 == phone2).Should().BeFalse();
    }
}
