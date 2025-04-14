using FluentValidation;

namespace EuroMotors.Application.Products.SetProductAvailability;

internal sealed class SetProductAvailabilityCommandValidator : AbstractValidator<SetProductAvailabilityCommand>
{
    public SetProductAvailabilityCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
