using FluentValidation;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
