using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.CreateClinicalNote;

public record CreateClinicalNoteCommand(
    Guid PatientId,
    Guid ProviderId,
    Guid? AppointmentId,
    NoteType NoteType,
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan) : IRequest<ApiResponse<ClinicalNoteDto>>;
