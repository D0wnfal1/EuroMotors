using FluentValidation;

namespace EuroMotors.Application.CarModels.DeleteCarModel;

internal sealed class DeleteCarModelCommandValidator : AbstractValidator<DeleteCarModelCommand>
{
    public DeleteCarModelCommandValidator()
    {
        RuleFor(c => c.CarModelId).NotEmpty();
    }
}
