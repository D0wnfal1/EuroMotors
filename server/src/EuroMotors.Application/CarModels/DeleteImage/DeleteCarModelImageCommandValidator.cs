using FluentValidation;

namespace EuroMotors.Application.CarModels.DeleteImage;

internal sealed class DeleteCategoryImageCommandValidator : AbstractValidator<DeleteCarModelImageCommand>
{
    public DeleteCategoryImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
