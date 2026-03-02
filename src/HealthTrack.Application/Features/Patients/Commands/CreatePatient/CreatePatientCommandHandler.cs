using AutoMapper;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.CreatePatient;

public sealed class CreatePatientCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreatePatientCommand, ApiResponse<PatientDto>>
{
    public async Task<ApiResponse<PatientDto>> Handle(
        CreatePatientCommand request,
        CancellationToken cancellationToken)
    {
        var existingPatient = await unitOfWork.Patients.GetByEmailAsync(request.Email, cancellationToken);
        if (existingPatient is not null)
            return ApiResponse<PatientDto>.FailureResponse("A patient with this email already exists.");

        var patient = new Patient
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = new DateOfBirth(request.DateOfBirth),
            Gender = request.Gender,
            Email = request.Email,
            Phone = new PhoneNumber(request.Phone),
            Address = new Address(request.Street, request.City, request.State, request.ZipCode, request.Country),
            MedicalHistory = request.MedicalHistory,
            Allergies = request.Allergies ?? [],
            Medications = request.Medications ?? [],
            CreatedBy = currentUserService.UserId
        };

        if (request.InsuranceProvider is not null && request.PolicyNumber is not null && request.GroupNumber is not null)
        {
            patient.InsuranceInfo = new InsuranceInfo(
                request.InsuranceProvider,
                request.PolicyNumber,
                request.GroupNumber,
                request.InsuranceExpiration);
        }

        if (request.EmergencyContactName is not null &&
            request.EmergencyContactRelationship is not null &&
            request.EmergencyContactPhone is not null)
        {
            patient.EmergencyContact = new EmergencyContact(
                request.EmergencyContactName,
                request.EmergencyContactRelationship,
                request.EmergencyContactPhone);
        }

        await unitOfWork.Patients.AddAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(dto, "Patient created successfully.");
    }
}
