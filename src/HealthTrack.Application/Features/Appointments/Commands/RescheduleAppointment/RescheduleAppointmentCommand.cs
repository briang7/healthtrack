using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.RescheduleAppointment;

public record RescheduleAppointmentCommand(Guid Id, DateTime NewTime) : IRequest<ApiResponse<AppointmentDto>>;
