using FluentAssertions;
using FluentValidation.TestHelper;
using HealthTrack.Application.Features.Auth.Commands.Login;

namespace HealthTrack.Application.Tests.Features.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var command = new LoginCommand("user@example.com", "SecurePassword123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Email_WhenEmptyOrNull_ShouldFailValidation(string? email)
    {
        // Arrange
        var command = new LoginCommand(email!, "SecurePassword123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Email_WhenInvalidFormat_ShouldFailValidation(string email)
    {
        // Arrange
        var command = new LoginCommand(email, "SecurePassword123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("A valid email address is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Password_WhenEmptyOrNull_ShouldFailValidation(string? password)
    {
        // Arrange
        var command = new LoginCommand("user@example.com", password!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required.");
    }

    [Fact]
    public void ValidEmail_WithValidPassword_ShouldPassAllRules()
    {
        // Arrange
        var command = new LoginCommand("admin@healthtrack.dev", "Demo123!");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
