﻿using FluentValidation;
using ProvidersMS.src.Drivers.Application.Commands.UpdateDriver.Types;

namespace ProvidersMS.src.Drivers.Infrastructure.Validators
{
    public class UpdateDriverValidator : AbstractValidator<UpdateDriverCommand>
    {
        public UpdateDriverValidator()
        {
            RuleFor(x => x.IsActiveLicensed)
                .NotNull().WithMessage("IsActiveLicensed must be true or false.")
                .When(x => x.IsActiveLicensed.HasValue);

            RuleFor(x => x.CraneAssigned)
                .NotNull().WithMessage("CraneAssigned must not be null.")
                .When(x => !string.IsNullOrEmpty(x.CraneAssigned));

            RuleFor(x => x.IsAvailable)
                .NotNull().WithMessage("IsAvailable must be true or false.")
                .When(x => x.IsAvailable.HasValue);

            RuleFor(x => x.DriverLocation)
               .NotNull().WithMessage("Driver location must not be null.")
               .When(x => !string.IsNullOrEmpty(x.DriverLocation));
        }
    }
}
