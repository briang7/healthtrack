using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.CreateClinicalNote;

public record CreateClinicalNoteCommand(
    Guid PatientId,
    Guid ProviderId,
    Guid? AppointmentId,
    NoteType NoteType,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Subjective,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Objective,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Assessment,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Plan) : IRequest<ApiResponse<ClinicalNoteDto>>;
