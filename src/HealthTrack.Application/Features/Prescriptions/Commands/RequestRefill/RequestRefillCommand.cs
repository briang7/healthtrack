using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.RequestRefill;

public record RequestRefillCommand(Guid Id) : IRequest<ApiResponse<PrescriptionDto>>;
