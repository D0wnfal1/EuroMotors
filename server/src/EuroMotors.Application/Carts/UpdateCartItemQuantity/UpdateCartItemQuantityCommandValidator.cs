using FluentValidation;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

internal sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
