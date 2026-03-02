using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.CancelAppointment;

public sealed class CancelAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<CancelAppointmentCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        CancelAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.Appointments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        if (appointment.Status == AppointmentStatus.Cancelled)
            return ApiResponse<bool>.FailureResponse("This appointment is already cancelled.");

        if (appointment.Status == AppointmentStatus.Completed)
            return ApiResponse<bool>.FailureResponse("Cannot cancel a completed appointment.");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancellationReason = request.Reason;
        appointment.ModifiedAt = DateTime.UtcNow;
        appointment.ModifiedBy = currentUserService.UserId;

        await unitOfWork.Appointments.UpdateAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Appointment cancelled successfully.");
    }
}
