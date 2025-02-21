using FluentValidation;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .When(c => c.SessionId == Guid.Empty);
        RuleFor(c => c.SessionId)
            .NotEqual(Guid.Empty)
            .When(c => c.UserId == Guid.Empty);
    }
}
