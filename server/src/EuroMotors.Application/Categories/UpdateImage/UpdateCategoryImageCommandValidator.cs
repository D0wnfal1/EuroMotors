using FluentValidation;

namespace EuroMotors.Application.Categories.UpdateImage;

internal sealed class UpdateCategoryImageCommandValidator : AbstractValidator<UpdateCategoryImageCommand>
{
    public UpdateCategoryImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Url).NotEmpty();
    }
}
