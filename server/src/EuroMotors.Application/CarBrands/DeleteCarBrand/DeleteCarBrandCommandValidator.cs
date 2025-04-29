using FluentValidation;

namespace EuroMotors.Application.CarBrands.DeleteCarBrand;

internal sealed class DeleteCarBrandCommandValidator : AbstractValidator<DeleteCarBrandCommand>
{
    public DeleteCarBrandCommandValidator()
    {
        RuleFor(c => c.CarBrandId).NotEmpty();
    }
}
