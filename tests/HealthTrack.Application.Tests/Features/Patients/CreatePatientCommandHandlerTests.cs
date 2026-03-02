using AutoMapper;
using FluentAssertions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Features.Patients.Commands.CreatePatient;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using Moq;

namespace HealthTrack.Application.Tests.Features.Patients;

public class CreatePatientCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreatePatientCommandHandler _handler;

    public CreatePatientCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _currentUserServiceMock.Setup(x => x.UserId).Returns("test-user-id");

        _handler = new CreatePatientCommandHandler(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _currentUserServiceMock.Object);
    }

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
    public async Task Handle_WithValidCommand_ShouldReturnSuccessWithPatientDto()
    {
        // Arrange
        var command = CreateValidCommand();
        var expectedDto = new PatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Phone = command.Phone,
            Gender = command.Gender,
            DateOfBirth = command.DateOfBirth,
            IsActive = true
        };

        _unitOfWorkMock
            .Setup(x => x.Patients.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        _unitOfWorkMock
            .Setup(x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<PatientDto>(It.IsAny<Patient>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
        result.Data.Email.Should().Be("john.doe@example.com");
        result.Message.Should().Be("Patient created successfully.");

        _unitOfWorkMock.Verify(
            x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingPatient = new Patient
        {
            FirstName = "Existing",
            LastName = "Patient",
            DateOfBirth = new Domain.ValueObjects.DateOfBirth(DateTime.UtcNow.AddYears(-25)),
            Email = command.Email,
            Phone = new Domain.ValueObjects.PhoneNumber("(555) 999-0000"),
            Address = new Domain.ValueObjects.Address("456 Oak St", "Springfield", "IL", "62702")
        };

        _unitOfWorkMock
            .Setup(x => x.Patients.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPatient);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("A patient with this email already exists.");

        _unitOfWorkMock.Verify(
            x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedByFromCurrentUser()
    {
        // Arrange
        var command = CreateValidCommand();
        Patient? capturedPatient = null;

        _unitOfWorkMock
            .Setup(x => x.Patients.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        _unitOfWorkMock
            .Setup(x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<PatientDto>(It.IsAny<Patient>()))
            .Returns(new PatientDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPatient.Should().NotBeNull();
        capturedPatient!.CreatedBy.Should().Be("test-user-id");
    }

    [Fact]
    public async Task Handle_WithInsuranceInfo_ShouldSetInsuranceOnPatient()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            InsuranceProvider = "BlueCross",
            PolicyNumber = "BC-12345",
            GroupNumber = "GRP-001",
            InsuranceExpiration = DateTime.UtcNow.AddYears(1)
        };

        Patient? capturedPatient = null;

        _unitOfWorkMock
            .Setup(x => x.Patients.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        _unitOfWorkMock
            .Setup(x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<PatientDto>(It.IsAny<Patient>()))
            .Returns(new PatientDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPatient.Should().NotBeNull();
        capturedPatient!.InsuranceInfo.Should().NotBeNull();
        capturedPatient.InsuranceInfo!.Provider.Should().Be("BlueCross");
        capturedPatient.InsuranceInfo.PolicyNumber.Should().Be("BC-12345");
        capturedPatient.InsuranceInfo.GroupNumber.Should().Be("GRP-001");
    }

    [Fact]
    public async Task Handle_WithEmergencyContact_ShouldSetEmergencyContactOnPatient()
    {
        // Arrange
        var command = CreateValidCommand() with
        {
            EmergencyContactName = "Jane Doe",
            EmergencyContactRelationship = "Spouse",
            EmergencyContactPhone = "(555) 999-8888"
        };

        Patient? capturedPatient = null;

        _unitOfWorkMock
            .Setup(x => x.Patients.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient?)null);

        _unitOfWorkMock
            .Setup(x => x.Patients.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .Callback<Patient, CancellationToken>((p, _) => capturedPatient = p)
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<PatientDto>(It.IsAny<Patient>()))
            .Returns(new PatientDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPatient.Should().NotBeNull();
        capturedPatient!.EmergencyContact.Should().NotBeNull();
        capturedPatient.EmergencyContact!.Name.Should().Be("Jane Doe");
        capturedPatient.EmergencyContact.Relationship.Should().Be("Spouse");
        capturedPatient.EmergencyContact.Phone.Should().Be("(555) 999-8888");
    }
}
