using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Attributes;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.AmendClinicalNote;

public record AmendClinicalNoteCommand(
    Guid Id,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Subjective,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Objective,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Assessment,
    [property: Phi(PhiSensitivity.HighlySensitive, Category = "Clinical")] string? Plan) : IRequest<ApiResponse<ClinicalNoteDto>>;
