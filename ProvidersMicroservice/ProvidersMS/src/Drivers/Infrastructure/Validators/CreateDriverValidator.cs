using FluentValidation;
using ProvidersMS.src.Drivers.Infrastructure.Dtos;

namespace ProvidersMS.src.Drivers.Infrastructure.Validators
{
    public class CreateDriverValidator : AbstractValidator<CreateDriverWithImagesCommand>
    {
        public CreateDriverValidator()
        {
            RuleFor(x => x.DNI)
            .NotEmpty().WithMessage("DNI is required.")
            .MinimumLength(7).WithMessage("The DNI must not be less than 7 characters.")
            .MaximumLength(8).WithMessage("DNI must be less than 8 characters.");

            RuleFor(x => x.IsActiveLicensed)
            .NotEmpty().WithMessage("IsActiveLicensed is required.");

            RuleFor(x => x.CraneAssigned)
            .NotNull().WithMessage("Crane assigned is required.")
            .When(x => !string.IsNullOrEmpty(x.CraneAssigned));

            RuleFor(x => x.DriverLocation)
            .NotEmpty().WithMessage("Driver location is required.");
        }
    }
}
