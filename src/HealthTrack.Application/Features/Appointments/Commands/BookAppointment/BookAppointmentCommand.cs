using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.BookAppointment;

public record BookAppointmentCommand(
    Guid PatientId,
    Guid ProviderId,
    DateTime ScheduledAt,
    TimeSpan Duration,
    AppointmentType Type,
    string? Notes,
    bool IsRecurring = false,
    string? RecurrencePattern = null) : IRequest<ApiResponse<AppointmentDto>>;
