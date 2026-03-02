using AutoMapper;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.CreatePharmacy;

public sealed class CreatePharmacyCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreatePharmacyCommand, ApiResponse<PharmacyDto>>
{
    public async Task<ApiResponse<PharmacyDto>> Handle(
        CreatePharmacyCommand request,
        CancellationToken cancellationToken)
    {
        var pharmacy = new Pharmacy
        {
            Name = request.Name,
            Address = new Address(request.Street, request.City, request.State, request.ZipCode, request.Country),
            Phone = new PhoneNumber(request.Phone),
            Fax = request.Fax,
            IsActive = true,
            CreatedBy = currentUserService.UserId
        };

        await unitOfWork.Pharmacies.AddAsync(pharmacy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PharmacyDto>(pharmacy);
        return ApiResponse<PharmacyDto>.SuccessResponse(dto, "Pharmacy created successfully.");
    }
}
