using FluentValidation;

namespace EuroMotors.Application.CarBrands.CreateCarBrand;

internal sealed class CreateCarBrandCommandValidator : AbstractValidator<CreateCarBrandCommand>
{
    public CreateCarBrandCommandValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Logo).NotEmpty()
            .When(c => c.Logo is not null);
    }
}
