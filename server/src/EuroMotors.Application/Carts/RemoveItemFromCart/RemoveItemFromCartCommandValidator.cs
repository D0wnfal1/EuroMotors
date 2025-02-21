using FluentValidation;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandValidator : AbstractValidator<RemoveItemFromCartCommand>
{
    public RemoveItemFromCartCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .When(c => c.SessionId == Guid.Empty);
        RuleFor(c => c.SessionId)
            .NotEqual(Guid.Empty)
            .When(c => c.UserId == Guid.Empty);
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
