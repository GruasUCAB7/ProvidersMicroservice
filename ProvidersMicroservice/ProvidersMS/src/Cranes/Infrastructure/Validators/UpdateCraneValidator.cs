using FluentValidation;
using ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types;

namespace ProvidersMS.src.Cranes.Infrastructure.Validators
{
    public class UpdateCraneValidator : AbstractValidator<UpdateCraneCommand>
    {
        public UpdateCraneValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive must be true or false.");

        }
    }
}
