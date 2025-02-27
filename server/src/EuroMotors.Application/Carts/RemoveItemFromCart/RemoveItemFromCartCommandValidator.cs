using FluentValidation;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandValidator : AbstractValidator<RemoveItemFromCartCommand>
{
    public RemoveItemFromCartCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
