using FluentValidation;

namespace HealthTrack.Application.Features.Pharmacies.Commands.CreatePharmacy;

public sealed class CreatePharmacyCommandValidator : AbstractValidator<CreatePharmacyCommand>
{
    public CreatePharmacyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pharmacy name is required.")
            .MaximumLength(200).WithMessage("Pharmacy name must not exceed 200 characters.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street address is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("ZIP code is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s\-\(\)]{7,15}$").WithMessage("A valid phone number is required.");
    }
}
