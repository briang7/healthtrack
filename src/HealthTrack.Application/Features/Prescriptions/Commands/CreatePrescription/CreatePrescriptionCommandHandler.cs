using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.CreatePrescription;

public sealed class CreatePrescriptionCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreatePrescriptionCommand, ApiResponse<PrescriptionDto>>
{
    public async Task<ApiResponse<PrescriptionDto>> Handle(
        CreatePrescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var provider = await unitOfWork.Providers.GetByIdAsync(request.ProviderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Provider), request.ProviderId);

        if (request.PharmacyId.HasValue)
        {
            _ = await unitOfWork.Pharmacies.GetByIdAsync(request.PharmacyId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Pharmacy), request.PharmacyId.Value);
        }

        var prescription = new Prescription
        {
            PatientId = request.PatientId,
            ProviderId = request.ProviderId,
            PharmacyId = request.PharmacyId,
            MedicationName = request.MedicationName,
            Dosage = request.Dosage,
            Frequency = request.Frequency,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RefillsRemaining = request.RefillsRemaining,
            Status = PrescriptionStatus.Active,
            Notes = request.Notes,
            CreatedBy = currentUserService.UserId,
            Patient = patient,
            Provider = provider
        };

        await unitOfWork.Prescriptions.AddAsync(prescription, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PrescriptionDto>(prescription);
        return ApiResponse<PrescriptionDto>.SuccessResponse(dto, "Prescription created successfully.");
    }
}
