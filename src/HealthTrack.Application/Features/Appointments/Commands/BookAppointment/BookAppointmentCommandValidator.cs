using FluentValidation;

namespace HealthTrack.Application.Features.Appointments.Commands.BookAppointment;

public sealed class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("Provider ID is required.");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment must be scheduled in the future.");

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Duration must be greater than zero.")
            .LessThanOrEqualTo(TimeSpan.FromHours(4)).WithMessage("Duration must not exceed 4 hours.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("A valid appointment type is required.");
    }
}
