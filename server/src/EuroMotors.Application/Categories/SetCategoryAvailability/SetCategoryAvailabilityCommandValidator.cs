using FluentValidation;

namespace EuroMotors.Application.Categories.SetCategoryAvailability;

internal sealed class SetCategoryAvailabilityCommandValidator : AbstractValidator<SetCategoryAvailabilityCommand>
{
    public SetCategoryAvailabilityCommandValidator()
    {
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}
