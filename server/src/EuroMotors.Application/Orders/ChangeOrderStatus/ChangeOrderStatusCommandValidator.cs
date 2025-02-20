using FluentValidation;

namespace EuroMotors.Application.Orders.ChangeOrderStatus;

internal sealed class ChangeOrderStatusCommandValidator : AbstractValidator<ChangeOrderStatusCommand>
{
    public ChangeOrderStatusCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty();
        RuleFor(o => o.Status).IsInEnum();
    }
}
