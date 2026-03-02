using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.AmendClinicalNote;

public record AmendClinicalNoteCommand(
    Guid Id,
    string? Subjective,
    string? Objective,
    string? Assessment,
    string? Plan) : IRequest<ApiResponse<ClinicalNoteDto>>;
