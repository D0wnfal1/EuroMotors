using FluentValidation;

namespace EuroMotors.Application.CarModels.DeleteImage;

internal sealed class DeleteCarModelImageCommandValidator : AbstractValidator<DeleteCarModelImageCommand>
{
    public DeleteCarModelImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
