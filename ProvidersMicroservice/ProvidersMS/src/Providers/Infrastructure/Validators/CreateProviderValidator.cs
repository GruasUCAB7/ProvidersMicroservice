using FluentValidation;
using ProvidersMS.src.Providers.Application.Commands.UpdateProvider.Types;
using ProvidersMS.src.Providers.Domain.ValueObjects;

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
        }
    }
}
