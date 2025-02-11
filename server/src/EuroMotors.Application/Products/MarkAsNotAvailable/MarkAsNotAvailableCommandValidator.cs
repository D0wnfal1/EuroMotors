using FluentValidation;

namespace EuroMotors.Application.Products.MarkAsNotAvailable;

internal sealed class MarkAsNotAvailableCommandValidator : AbstractValidator<MarkAsNotAvailableCommand>
{
    public MarkAsNotAvailableCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
