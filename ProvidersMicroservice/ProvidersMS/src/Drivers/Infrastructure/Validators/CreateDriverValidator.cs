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
            .NotEmpty().WithMessage("Crane id is required.");
        }
    }
}
