using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Models;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.DeletePharmacy;

public sealed class DeletePharmacyCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePharmacyCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeletePharmacyCommand request,
        CancellationToken cancellationToken)
    {
        var pharmacy = await unitOfWork.Pharmacies.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Pharmacy), request.Id);

        await unitOfWork.Pharmacies.DeleteAsync(pharmacy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Pharmacy deleted successfully.");
    }
}
