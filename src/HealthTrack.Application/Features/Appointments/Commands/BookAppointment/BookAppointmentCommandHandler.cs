using AutoMapper;
using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Commands.BookAppointment;

public sealed class BookAppointmentCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICurrentUserService currentUserService)
    : IRequestHandler<BookAppointmentCommand, ApiResponse<AppointmentDto>>
{
    public async Task<ApiResponse<AppointmentDto>> Handle(
        BookAppointmentCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await unitOfWork.Patients.GetByIdAsync(request.PatientId, cancellationToken)
            ?? throw new NotFoundException(nameof(Patient), request.PatientId);

        var provider = await unitOfWork.Providers.GetByIdAsync(request.ProviderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Provider), request.ProviderId);

        if (!provider.IsAcceptingPatients)
            return ApiResponse<AppointmentDto>.FailureResponse("This provider is not currently accepting patients.");

        // Check for scheduling conflicts
        var existingAppointments = await unitOfWork.Appointments
            .GetByProviderIdAsync(request.ProviderId, cancellationToken);

        var hasConflict = existingAppointments.Any(a =>
            a.Status != AppointmentStatus.Cancelled &&
            a.ScheduledAt < request.ScheduledAt.Add(request.Duration) &&
            a.ScheduledAt.Add(a.Duration) > request.ScheduledAt);

        if (hasConflict)
            return ApiResponse<AppointmentDto>.FailureResponse("The selected time slot conflicts with an existing appointment.");

        var appointment = new Appointment
        {
            PatientId = request.PatientId,
            ProviderId = request.ProviderId,
            ScheduledAt = request.ScheduledAt,
            Duration = request.Duration,
            Type = request.Type,
            Notes = request.Notes,
            Status = AppointmentStatus.Scheduled,
            IsRecurring = request.IsRecurring,
            RecurrencePattern = request.RecurrencePattern,
            CreatedBy = currentUserService.UserId,
            Patient = patient,
            Provider = provider
        };

        await unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<AppointmentDto>(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(dto, "Appointment booked successfully.");
    }
}
