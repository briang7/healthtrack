using FluentValidation;

namespace HealthTrack.Application.Features.ClinicalNotes.Commands.CreateClinicalNote;

public sealed class CreateClinicalNoteCommandValidator : AbstractValidator<CreateClinicalNoteCommand>
{
    public CreateClinicalNoteCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.ProviderId)
            .NotEmpty().WithMessage("Provider ID is required.");

        RuleFor(x => x.NoteType)
            .IsInEnum().WithMessage("A valid note type is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Subjective) ||
                        !string.IsNullOrWhiteSpace(x.Objective) ||
                        !string.IsNullOrWhiteSpace(x.Assessment) ||
                        !string.IsNullOrWhiteSpace(x.Plan))
            .WithMessage("At least one SOAP field (Subjective, Objective, Assessment, or Plan) is required.");
    }
}
