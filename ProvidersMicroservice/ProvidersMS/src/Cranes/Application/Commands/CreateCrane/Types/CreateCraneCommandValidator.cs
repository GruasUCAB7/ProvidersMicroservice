using FluentValidation;

namespace ProvidersMS.src.Cranes.Application.Commands.CreateCrane.Types
{
    public class CreateCraneCommandValidator : AbstractValidator<CreateCraneCommand>
    {
        public CreateCraneCommandValidator()
        {
            RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MinimumLength(2).WithMessage("The brand must not be less than 2 characters.")
            .MaximumLength(20).WithMessage("Brand must be less than 20 characters.");

            RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MinimumLength(2).WithMessage("The model must not be less than 2 characters.")
            .MaximumLength(20).WithMessage("Model must be less than 20 characters.");

            RuleFor(x => x.Plate)
            .NotEmpty().WithMessage("Plate is required.");

            RuleFor(x => x.CraneType)
            .NotEmpty().WithMessage("Crane type is required.");

            RuleFor(x => x.Year)
            .NotEmpty().WithMessage("Year of de crane is required.");
        }
    }
}
