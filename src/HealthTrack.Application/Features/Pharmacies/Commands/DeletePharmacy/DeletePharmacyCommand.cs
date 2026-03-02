using HealthTrack.Application.Common.Models;
using MediatR;

namespace HealthTrack.Application.Features.Pharmacies.Commands.DeletePharmacy;

public record DeletePharmacyCommand(Guid Id) : IRequest<ApiResponse<bool>>;
