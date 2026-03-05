using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.CreatePatient;

public record CreatePatientCommand(
    [property: Phi(PhiSensitivity.Standard, Category = "Demographics")] string FirstName,
    [property: Phi(PhiSensitivity.Standard, Category = "Demographics")] string LastName,
    [property: Phi(PhiSensitivity.Standard, Category = "Demographics")] DateTime DateOfBirth,
    Gender Gender,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string Email,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string Phone,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string Street,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string City,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string State,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string ZipCode,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string Country,
    [property: Phi(PhiSensitivity.Standard, Category = "Financial")] string? InsuranceProvider,
    [property: Phi(PhiSensitivity.Standard, Category = "Financial")] string? PolicyNumber,
    [property: Phi(PhiSensitivity.Standard, Category = "Financial")] string? GroupNumber,
    [property: Phi(PhiSensitivity.Standard, Category = "Financial")] DateTime? InsuranceExpiration,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string? EmergencyContactName,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string? EmergencyContactRelationship,
    [property: Phi(PhiSensitivity.Standard, Category = "Contact")] string? EmergencyContactPhone,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string? MedicalHistory,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] List<string>? Allergies,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] List<string>? Medications) : IRequest<ApiResponse<PatientDto>>;
