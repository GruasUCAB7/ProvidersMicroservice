using FluentValidation;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;

namespace ProvidersMS.src.Providers.Infrastructure.Validators
{
    public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
    {
        public UpdateProviderValidator()
        {
            RuleFor(x => x.FleetOfCranes)
            .NotNull().WithMessage("FleetOfCranes is required.")
            .When(x => x.FleetOfCranes != null);

            RuleFor(x => x.Drivers)
            .NotNull().WithMessage("Drivers is required.")
            .When(x => x.Drivers != null);

            RuleFor(x => x.IsActive)
                .NotNull().WithMessage("IsActive must be true or false.")
                .When(x => x.IsActive.HasValue);
        }
    }
}
