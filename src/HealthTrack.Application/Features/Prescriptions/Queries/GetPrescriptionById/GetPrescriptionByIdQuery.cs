using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptionById;

public record GetPrescriptionByIdQuery(Guid Id) : IRequest<ApiResponse<PrescriptionDto>>;
