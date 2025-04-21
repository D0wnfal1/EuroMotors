using EuroMotors.Application.CarBrands.CreateCarBrand;
using FluentValidation;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class CreateCarBrandCommandValidator : AbstractValidator<CreateCarBrandCommand>
{
    public CreateCarBrandCommandValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Logo).NotEmpty()
            .When(c => c.Logo is not null);
    }
}
