using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptions;

public record GetPrescriptionsQuery(
    Guid? PatientId = null,
    PrescriptionStatus? Status = null,
    int Page = 1,
    int PageSize = 10) : IRequest<ApiResponse<PagedResult<PrescriptionDto>>>;
