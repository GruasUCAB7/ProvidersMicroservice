using FluentValidation;

namespace ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types
{
    public class UpdateCraneCommandValidator : AbstractValidator<UpdateCraneCommand>
    {
        public UpdateCraneCommandValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive must be true or false.");

        }
    }
}
