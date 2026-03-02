using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Commands.UpdateProvider;

public sealed class UpdateProviderCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateProviderCommand, ApiResponse<ProviderDto>>
{
    public async Task<ApiResponse<ProviderDto>> Handle(
        UpdateProviderCommand request,
        CancellationToken cancellationToken)
    {
        var provider = await unitOfWork.Providers.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Provider), request.Id);

        provider.FirstName = request.FirstName;
        provider.LastName = request.LastName;
        provider.Specialty = request.Specialty;
        provider.LicenseNumber = request.LicenseNumber;
        provider.Email = request.Email;
        provider.Phone = new PhoneNumber(request.Phone);
        provider.IsAcceptingPatients = request.IsAcceptingPatients;
        provider.ModifiedAt = DateTime.UtcNow;
        provider.ModifiedBy = currentUserService.UserId;

        await unitOfWork.Providers.UpdateAsync(provider, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ProviderDto>(provider);
        return ApiResponse<ProviderDto>.SuccessResponse(dto, "Provider updated successfully.");
    }
}
