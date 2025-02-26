using FluentValidation;

namespace EuroMotors.Application.Categories.DeleteImage;

internal sealed class DeleteCategoryImageCommandValidator : AbstractValidator<DeleteCategoryImageCommand>
{
    public DeleteCategoryImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
