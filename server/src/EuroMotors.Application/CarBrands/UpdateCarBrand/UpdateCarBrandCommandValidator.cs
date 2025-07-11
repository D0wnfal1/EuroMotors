﻿using FluentValidation;

namespace EuroMotors.Application.CarBrands.UpdateCarBrand;

internal sealed class UpdateCarBrandCommandValidator : AbstractValidator<UpdateCarBrandCommand>
{
    public UpdateCarBrandCommandValidator()
    {
        RuleFor(c => c.CarBrandId).NotEmpty();
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Logo).NotEmpty()
            .When(c => c.Logo is not null);
    }
}
