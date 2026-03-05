using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Attributes;
using HealthTrack.Domain.Enums;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.BookAppointment;

public record BookAppointmentCommand(
    Guid PatientId,
    Guid ProviderId,
    DateTime ScheduledAt,
    TimeSpan Duration,
    AppointmentType Type,
    [property: Phi(PhiSensitivity.Sensitive, Category = "Clinical")] string? Notes,
    bool IsRecurring = false,
    string? RecurrencePattern = null) : IRequest<ApiResponse<AppointmentDto>>;
