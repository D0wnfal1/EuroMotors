using FluentValidation;

namespace EuroMotors.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.Quantity).GreaterThan(0);
    }
}
