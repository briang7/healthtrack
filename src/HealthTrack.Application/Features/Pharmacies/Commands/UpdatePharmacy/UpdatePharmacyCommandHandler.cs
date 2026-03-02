using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Pharmacies.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.ValueObjects;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.UpdatePharmacy;

public sealed class UpdatePharmacyCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdatePharmacyCommand, ApiResponse<PharmacyDto>>
{
    public async Task<ApiResponse<PharmacyDto>> Handle(
        UpdatePharmacyCommand request,
        CancellationToken cancellationToken)
    {
        var pharmacy = await unitOfWork.Pharmacies.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Pharmacy), request.Id);

        pharmacy.Name = request.Name;
        pharmacy.Address = new Address(request.Street, request.City, request.State, request.ZipCode, request.Country);
        pharmacy.Phone = new PhoneNumber(request.Phone);
        pharmacy.Fax = request.Fax;
        pharmacy.IsActive = request.IsActive;
        pharmacy.ModifiedAt = DateTime.UtcNow;
        pharmacy.ModifiedBy = currentUserService.UserId;

        await unitOfWork.Pharmacies.UpdateAsync(pharmacy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<PharmacyDto>(pharmacy);
        return ApiResponse<PharmacyDto>.SuccessResponse(dto, "Pharmacy updated successfully.");
    }
}
