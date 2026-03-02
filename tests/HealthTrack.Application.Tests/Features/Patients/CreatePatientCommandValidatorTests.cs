using FluentAssertions;
using FluentValidation.TestHelper;
using HealthTrack.Application.Features.Patients.Commands.CreatePatient;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Tests.Features.Patients;

public class CreatePatientCommandValidatorTests
{
    private readonly CreatePatientCommandValidator _validator = new();

    private static CreatePatientCommand CreateValidCommand() => new(
        FirstName: "John",
        LastName: "Doe",
        DateOfBirth: DateTime.UtcNow.AddYears(-30),
        Gender: Gender.Male,
        Email: "john.doe@example.com",
        Phone: "(555) 123-4567",
        Street: "123 Main St",
        City: "Springfield",
        State: "IL",
        ZipCode: "62701",
        Country: "US",
        InsuranceProvider: null,
        PolicyNumber: null,
        GroupNumber: null,
        InsuranceExpiration: null,
        EmergencyContactName: null,
        EmergencyContactRelationship: null,
        EmergencyContactPhone: null,
        MedicalHistory: null,
        Allergies: null,
        Medications: null);

    [Fact]
    public void ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void FirstName_WhenEmptyOrNull_ShouldFailValidation(string? firstName)
    {
        // Arrange
        var command = CreateValidCommand() with { FirstName = firstName! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required.");
    }

    [Fact]
    public void FirstName_WhenExceeds100Characters_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { FirstName = new string('A', 101) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void LastName_WhenEmptyOrNull_ShouldFailValidation(string? lastName)
    {
        // Arrange
        var command = CreateValidCommand() with { LastName = lastName! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Fact]
    public void LastName_WhenExceeds100Characters_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { LastName = new string('B', 101) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Email_WhenEmptyOrNull_ShouldFailValidation(string? email)
    {
        // Arrange
        var command = CreateValidCommand() with { Email = email! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Email_WhenInvalidFormat_ShouldFailValidation(string email)
    {
        // Arrange
        var command = CreateValidCommand() with { Email = email };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void DateOfBirth_WhenInFuture_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { DateOfBirth = DateTime.UtcNow.AddDays(1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth must be in the past.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Phone_WhenEmptyOrNull_ShouldFailValidation(string? phone)
    {
        // Arrange
        var command = CreateValidCommand() with { Phone = phone! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Phone number is required.");
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12")]
    public void Phone_WhenInvalidFormat_ShouldFailValidation(string phone)
    {
        // Arrange
        var command = CreateValidCommand() with { Phone = phone };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("A valid phone number is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Street_WhenEmptyOrNull_ShouldFailValidation(string? street)
    {
        // Arrange
        var command = CreateValidCommand() with { Street = street! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Street)
            .WithErrorMessage("Street address is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void City_WhenEmptyOrNull_ShouldFailValidation(string? city)
    {
        // Arrange
        var command = CreateValidCommand() with { City = city! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.City)
            .WithErrorMessage("City is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void State_WhenEmptyOrNull_ShouldFailValidation(string? state)
    {
        // Arrange
        var command = CreateValidCommand() with { State = state! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.State)
            .WithErrorMessage("State is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ZipCode_WhenEmptyOrNull_ShouldFailValidation(string? zipCode)
    {
        // Arrange
        var command = CreateValidCommand() with { ZipCode = zipCode! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ZipCode)
            .WithErrorMessage("ZIP code is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Country_WhenEmptyOrNull_ShouldFailValidation(string? country)
    {
        // Arrange
        var command = CreateValidCommand() with { Country = country! };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Country)
            .WithErrorMessage("Country is required.");
    }

    [Fact]
    public void Gender_WhenInvalidEnumValue_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Gender = (Gender)999 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Gender)
            .WithErrorMessage("A valid gender value is required.");
    }
}
