using FluentValidation;

namespace EuroMotors.Application.Carts.ConvertToOrder;

internal sealed class ConvertToOrderCommandValidator : AbstractValidator<ConvertToOrderCommand>
{
    public ConvertToOrderCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty)
            .When(c => c.SessionId == Guid.Empty);
        RuleFor(c => c.SessionId)
            .NotEqual(Guid.Empty)
            .When(c => c.UserId == Guid.Empty);
    }
}
