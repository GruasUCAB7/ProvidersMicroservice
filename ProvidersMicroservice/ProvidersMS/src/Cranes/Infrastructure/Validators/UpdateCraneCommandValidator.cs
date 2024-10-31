using FluentValidation;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;

namespace ProvidersMS.src.Cranes.Infrastructure.Validators
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
