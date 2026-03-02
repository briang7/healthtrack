using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.RescheduleAppointment;

public sealed class RescheduleAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<RescheduleAppointmentCommand, ApiResponse<AppointmentDto>>
{
    public async Task<ApiResponse<AppointmentDto>> Handle(
        RescheduleAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        if (appointment.Status == AppointmentStatus.Cancelled)
            return ApiResponse<AppointmentDto>.FailureResponse("Cannot reschedule a cancelled appointment.");

        if (appointment.Status == AppointmentStatus.Completed)
            return ApiResponse<AppointmentDto>.FailureResponse("Cannot reschedule a completed appointment.");

        // Check for scheduling conflicts at the new time
        var existingAppointments = await unitOfWork.Appointments
            .GetByProviderIdAsync(appointment.ProviderId, cancellationToken);

        var hasConflict = existingAppointments.Any(a =>
            a.Id != appointment.Id &&
            a.Status != AppointmentStatus.Cancelled &&
            a.ScheduledAt < request.NewTime.Add(appointment.Duration) &&
            a.ScheduledAt.Add(a.Duration) > request.NewTime);

        if (hasConflict)
            return ApiResponse<AppointmentDto>.FailureResponse("The new time slot conflicts with an existing appointment.");

        appointment.ScheduledAt = request.NewTime;
        appointment.Status = AppointmentStatus.Rescheduled;
        appointment.ModifiedAt = DateTime.UtcNow;
        appointment.ModifiedBy = currentUserService.UserId;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<AppointmentDto>(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(dto, "Appointment rescheduled successfully.");
    }
}
