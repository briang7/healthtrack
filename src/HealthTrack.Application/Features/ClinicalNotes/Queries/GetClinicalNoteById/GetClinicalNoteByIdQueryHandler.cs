using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNoteById;

public sealed class GetClinicalNoteByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetClinicalNoteByIdQuery, ApiResponse<ClinicalNoteDto>>
{
    public async Task<ApiResponse<ClinicalNoteDto>> Handle(
        GetClinicalNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var note = await unitOfWork.ClinicalNotes.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ClinicalNote), request.Id);

        var dto = mapper.Map<ClinicalNoteDto>(note);
        return ApiResponse<ClinicalNoteDto>.SuccessResponse(dto);
    }
}
