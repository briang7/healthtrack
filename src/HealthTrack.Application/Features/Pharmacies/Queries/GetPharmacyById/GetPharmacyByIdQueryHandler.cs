using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Queries.GetPharmacyById;

public sealed class GetPharmacyByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetPharmacyByIdQuery, ApiResponse<PharmacyDto>>
{
    public async Task<ApiResponse<PharmacyDto>> Handle(
        GetPharmacyByIdQuery request,
        CancellationToken cancellationToken)
    {
        var pharmacy = await unitOfWork.Pharmacies.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Pharmacy), request.Id);

        var dto = mapper.Map<PharmacyDto>(pharmacy);
        return ApiResponse<PharmacyDto>.SuccessResponse(dto);
    }
}
