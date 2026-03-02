using HealthTrack.Application.Common.Models;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid Id, string Reason) : IRequest<ApiResponse<bool>>;
