using EuroMotors.Application.CarBrands.DeleteCarBrand;
using FluentValidation;

namespace EuroMotors.Application.CarModels.CreateCarModel;

internal sealed class DeleteCarBrandCommandValidator : AbstractValidator<DeleteCarBrandCommand>
{
    public DeleteCarBrandCommandValidator()
    {
        RuleFor(c => c.CarBrandId).NotEmpty();
    }
}
