using FluentValidation;

namespace HealthTrack.Application.Features.Appointments.Commands.RescheduleAppointment;

public sealed class RescheduleAppointmentCommandValidator : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(x => x.NewTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("New appointment time must be in the future.");
    }
}
