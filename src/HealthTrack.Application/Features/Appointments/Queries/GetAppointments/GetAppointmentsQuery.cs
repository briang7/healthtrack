using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Queries.GetAppointments;

public record GetAppointmentsQuery(
    Guid? PatientId = null,
    Guid? ProviderId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    AppointmentStatus? Status = null,
    int Page = 1,
    int PageSize = 10) : IRequest<ApiResponse<PagedResult<AppointmentDto>>>;
