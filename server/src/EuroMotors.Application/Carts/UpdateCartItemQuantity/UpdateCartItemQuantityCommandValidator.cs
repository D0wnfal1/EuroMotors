using FluentValidation;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

internal sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .When(c => c.SessionId == Guid.Empty);
        RuleFor(c => c.SessionId)
            .NotEqual(Guid.Empty)
            .When(c => c.UserId == Guid.Empty);
        RuleFor(x => x.NewQuantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
