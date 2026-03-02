using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.UpdatePatient;

public sealed class UpdatePatientCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdatePatientCommand, ApiResponse<PatientDto>>
{
    public async Task<ApiResponse<PatientDto>> Handle(
        UpdatePatientCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.Id);

        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.DateOfBirth = new DateOfBirth(request.DateOfBirth);
        patient.Gender = request.Gender;
        patient.Email = request.Email;
        patient.Phone = new PhoneNumber(request.Phone);
        patient.Address = new Address(request.Street, request.City, request.State, request.ZipCode, request.Country);
        patient.MedicalHistory = request.MedicalHistory;
        patient.Allergies = request.Allergies ?? [];
        patient.Medications = request.Medications ?? [];
        patient.IsActive = request.IsActive;
        patient.ModifiedAt = DateTime.UtcNow;
        patient.ModifiedBy = currentUserService.UserId;

        if (request.InsuranceProvider is not null && request.PolicyNumber is not null && request.GroupNumber is not null)
        {
            patient.InsuranceInfo = new InsuranceInfo(
                request.InsuranceProvider,
                request.PolicyNumber,
                request.GroupNumber,
                request.InsuranceExpiration);
        }
        else
        {
            patient.InsuranceInfo = null;
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
        else
        {
            patient.EmergencyContact = null;
        }

        await unitOfWork.Patients.UpdateAsync(patient, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(dto, "Patient updated successfully.");
    }
}
