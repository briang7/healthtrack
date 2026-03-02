using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.CreateClinicalNote;

public sealed class CreateClinicalNoteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateClinicalNoteCommand, ApiResponse<ClinicalNoteDto>>
{
    public async Task<ApiResponse<ClinicalNoteDto>> Handle(
        CreateClinicalNoteCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var provider = await unitOfWork.Providers.GetByIdAsync(request.ProviderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Provider), request.ProviderId);

        if (request.AppointmentId.HasValue)
        {
            _ = await unitOfWork.Appointments.GetByIdAsync(request.AppointmentId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Appointment), request.AppointmentId.Value);
        }

        var clinicalNote = new ClinicalNote
        {
            PatientId = request.PatientId,
            ProviderId = request.ProviderId,
            AppointmentId = request.AppointmentId,
            NoteType = request.NoteType,
            Subjective = request.Subjective,
            Objective = request.Objective,
            Assessment = request.Assessment,
            Plan = request.Plan,
            Version = 1,
            IsAmended = false,
            CreatedBy = currentUserService.UserId,
            Patient = patient,
            Provider = provider
        };

        await unitOfWork.ClinicalNotes.AddAsync(clinicalNote, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ClinicalNoteDto>(clinicalNote);
        return ApiResponse<ClinicalNoteDto>.SuccessResponse(dto, "Clinical note created successfully.");
    }
}
