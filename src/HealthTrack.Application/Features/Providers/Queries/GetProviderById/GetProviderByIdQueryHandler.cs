using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Queries.GetProviderById;

public sealed class GetProviderByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetProviderByIdQuery, ApiResponse<ProviderDetailDto>>
{
    public async Task<ApiResponse<ProviderDetailDto>> Handle(
        GetProviderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var provider = await unitOfWork.Providers.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Provider), request.Id);

        var dto = mapper.Map<ProviderDetailDto>(provider);
        return ApiResponse<ProviderDetailDto>.SuccessResponse(dto);
    }
}
