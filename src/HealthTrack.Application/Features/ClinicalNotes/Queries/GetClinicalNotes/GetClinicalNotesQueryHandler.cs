using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNotes;

public sealed class GetClinicalNotesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetClinicalNotesQuery, ApiResponse<PagedResult<ClinicalNoteDto>>>
{
    public async Task<ApiResponse<PagedResult<ClinicalNoteDto>>> Handle(
        GetClinicalNotesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ClinicalNote> notes;

        if (request.PatientId.HasValue)
        {
            notes = await unitOfWork.ClinicalNotes
                .GetByPatientIdAsync(request.PatientId.Value, cancellationToken);
        }
        else
        {
            notes = await unitOfWork.ClinicalNotes.GetAllAsync(cancellationToken);
        }

        var filtered = notes.AsEnumerable();

        if (request.ProviderId.HasValue)
            filtered = filtered.Where(n => n.ProviderId == request.ProviderId.Value);

        var filteredList = filtered.OrderByDescending(n => n.CreatedAt).ToList();
        var totalCount = filteredList.Count;

        var paged = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<ClinicalNoteDto>>(paged);

        var result = new PagedResult<ClinicalNoteDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<ClinicalNoteDto>>.SuccessResponse(result);
    }
}
