using AutoMapper;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Providers.Commands.CreateProvider;

public sealed class CreateProviderCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateProviderCommand, ApiResponse<ProviderDto>>
{
    public async Task<ApiResponse<ProviderDto>> Handle(
        CreateProviderCommand request,
        CancellationToken cancellationToken)
    {
        var provider = new Provider
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Specialty = request.Specialty,
            LicenseNumber = request.LicenseNumber,
            Email = request.Email,
            Phone = new PhoneNumber(request.Phone),
            IsAcceptingPatients = request.IsAcceptingPatients,
            CreatedBy = currentUserService.UserId
        };

        await unitOfWork.Providers.AddAsync(provider, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<ProviderDto>(provider);
        return ApiResponse<ProviderDto>.SuccessResponse(dto, "Provider created successfully.");
    }
}
