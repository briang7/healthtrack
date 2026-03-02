using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Queries.GetAppointmentById;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<ApiResponse<AppointmentDto>>;
