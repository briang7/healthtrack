using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.UpdatePatient;

public record UpdatePatientCommand(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string Email,
    string Phone,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? InsuranceProvider,
    string? PolicyNumber,
    string? GroupNumber,
    DateTime? InsuranceExpiration,
    string? EmergencyContactName,
    string? EmergencyContactRelationship,
    string? EmergencyContactPhone,
    string? MedicalHistory,
    List<string>? Allergies,
    List<string>? Medications,
    bool IsActive) : IRequest<ApiResponse<PatientDto>>;
