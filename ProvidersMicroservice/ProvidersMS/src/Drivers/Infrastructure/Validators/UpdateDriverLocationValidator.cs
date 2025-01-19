using FluentValidation;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriverLocation.Types;

namespace ProvidersMS.src.Drivers.Infrastructure.Validators
{
    public class UpdateDriverLocationValidator : AbstractValidator<UpdateDriverLocationCommand>
    {
        public UpdateDriverLocationValidator()
        {
            RuleFor(x => x.Latitude)
                .NotNull().WithMessage("Latitude must not be null.")
                .GreaterThan(-90).WithMessage("Latitude must be greater than -90.")
                .LessThan(90).WithMessage("Latitude must be less than 90.");

            RuleFor(x => x.Longitude)
                .NotNull().WithMessage("Longitude must not be null.")
                .GreaterThan(-180).WithMessage("Longitude must be greater than -180.")
                .LessThan(180).WithMessage("Longitude must be less than 180.");
        }
    }

    


}
