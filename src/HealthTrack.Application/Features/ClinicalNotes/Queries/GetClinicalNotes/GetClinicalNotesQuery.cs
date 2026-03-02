using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNotes;

public record GetClinicalNotesQuery(
    Guid? PatientId = null,
    Guid? ProviderId = null,
    int Page = 1,
    int PageSize = 10) : IRequest<ApiResponse<PagedResult<ClinicalNoteDto>>>;
