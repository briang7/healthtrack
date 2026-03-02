using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNoteById;

public record GetClinicalNoteByIdQuery(Guid Id) : IRequest<ApiResponse<ClinicalNoteDto>>;
