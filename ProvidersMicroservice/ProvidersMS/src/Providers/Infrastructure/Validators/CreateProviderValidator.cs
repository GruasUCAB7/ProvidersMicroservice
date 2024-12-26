using FluentValidation;
using ProvidersMS.src.Providers.Application.Commands.CreateProvider.Types;
using ProvidersMS.src.Providers.Domain.ValueObjects;

namespace ProvidersMS.src.Providers.Infrastructure.Validators
{
    public class CreateProviderValidator : AbstractValidator<CreateProviderCommand>
    {
        public CreateProviderValidator()
        {
            RuleFor(x => x.Rif)
            .NotEmpty().WithMessage("Rif is required.")
            .Matches(@"^[JGVEP][0-9]{9}$").WithMessage("Rif must be in the format X123456789");

            RuleFor(x => x.ProviderType)
            .NotEmpty().WithMessage("ProviderType is required.")
            .IsEnumName(typeof(ProviderType), caseSensitive: false).WithMessage("ProviderType is not valid.");

            RuleFor(x => x.FleetOfCranes)
            .NotNull().WithMessage("FleetOfCranes is required.")
            .When(x => x.FleetOfCranes != null);

            RuleFor(x => x.Drivers)
            .NotNull().WithMessage("Drivers is required.")
            .When(x => x.Drivers != null);
        }
    }
}
