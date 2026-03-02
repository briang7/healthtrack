using FluentAssertions;
using FluentValidation.TestHelper;
using HealthTrack.Application.Features.Appointments.Commands.BookAppointment;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Application.Tests.Features.Appointments;

public class BookAppointmentCommandValidatorTests
{
    private readonly BookAppointmentCommandValidator _validator = new();

    private static BookAppointmentCommand CreateValidCommand() => new(
        PatientId: Guid.NewGuid(),
        ProviderId: Guid.NewGuid(),
        ScheduledAt: DateTime.UtcNow.AddDays(7),
        Duration: TimeSpan.FromMinutes(30),
        Type: AppointmentType.InPerson,
        Notes: "Routine checkup");

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

    [Fact]
    public void PatientId_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { PatientId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PatientId)
            .WithErrorMessage("Patient ID is required.");
    }

    [Fact]
    public void ProviderId_WhenEmpty_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { ProviderId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProviderId)
            .WithErrorMessage("Provider ID is required.");
    }

    [Fact]
    public void ScheduledAt_WhenInPast_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { ScheduledAt = DateTime.UtcNow.AddHours(-1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ScheduledAt)
            .WithErrorMessage("Appointment must be scheduled in the future.");
    }

    [Fact]
    public void Duration_WhenZero_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Duration = TimeSpan.Zero };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration)
            .WithErrorMessage("Duration must be greater than zero.");
    }

    [Fact]
    public void Duration_WhenNegative_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Duration = TimeSpan.FromMinutes(-15) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public void Duration_WhenExceedsFourHours_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Duration = TimeSpan.FromHours(5) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration)
            .WithErrorMessage("Duration must not exceed 4 hours.");
    }

    [Fact]
    public void Duration_AtExactlyFourHours_ShouldPassValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Duration = TimeSpan.FromHours(4) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public void Type_WhenInvalidEnumValue_ShouldFailValidation()
    {
        // Arrange
        var command = CreateValidCommand() with { Type = (AppointmentType)999 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage("A valid appointment type is required.");
    }

    [Theory]
    [InlineData(AppointmentType.InPerson)]
    [InlineData(AppointmentType.Telehealth)]
    [InlineData(AppointmentType.PhoneCall)]
    [InlineData(AppointmentType.FollowUp)]
    [InlineData(AppointmentType.Emergency)]
    [InlineData(AppointmentType.Consultation)]
    [InlineData(AppointmentType.Routine)]
    public void Type_WhenValidEnumValue_ShouldPassValidation(AppointmentType type)
    {
        // Arrange
        var command = CreateValidCommand() with { Type = type };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }
}
