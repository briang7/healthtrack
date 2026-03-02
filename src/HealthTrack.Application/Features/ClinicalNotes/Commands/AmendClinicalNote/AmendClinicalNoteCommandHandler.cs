using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.AmendClinicalNote;

public sealed class AmendClinicalNoteCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<AmendClinicalNoteCommand, ApiResponse<ClinicalNoteDto>>
{
    public async Task<ApiResponse<ClinicalNoteDto>> Handle(
        AmendClinicalNoteCommand request,
        CancellationToken cancellationToken)
    {
        var originalNote = await unitOfWork.ClinicalNotes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ClinicalNote), request.Id);

        // Mark original as amended
        originalNote.IsAmended = true;
        originalNote.ModifiedAt = DateTime.UtcNow;
        originalNote.ModifiedBy = currentUserService.UserId;
        await unitOfWork.ClinicalNotes.UpdateAsync(originalNote, cancellationToken);

        // Create new version linked to previous
        var amendedNote = new ClinicalNote
        {
            PatientId = originalNote.PatientId,
            ProviderId = originalNote.ProviderId,
            AppointmentId = originalNote.AppointmentId,
            NoteType = originalNote.NoteType,
            Subjective = request.Subjective ?? originalNote.Subjective,
            Objective = request.Objective ?? originalNote.Objective,
            Assessment = request.Assessment ?? originalNote.Assessment,
            Plan = request.Plan ?? originalNote.Plan,
            Version = originalNote.Version + 1,
            IsAmended = false,
            PreviousVersionId = originalNote.Id,
            CreatedBy = currentUserService.UserId
        };

        await unitOfWork.ClinicalNotes.AddAsync(amendedNote, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Load navigation properties for mapping
        amendedNote.Patient = originalNote.Patient;
        amendedNote.Provider = originalNote.Provider;

        var dto = mapper.Map<ClinicalNoteDto>(amendedNote);
        return ApiResponse<ClinicalNoteDto>.SuccessResponse(dto, "Clinical note amended successfully.");
    }
}
