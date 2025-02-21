using FluentValidation;

namespace EuroMotors.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .When(c => c.SessionId == Guid.Empty);
        RuleFor(c => c.SessionId)
            .NotEqual(Guid.Empty)
            .When(c => c.UserId == Guid.Empty);
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.Quantity).GreaterThan(0);
    }
}
