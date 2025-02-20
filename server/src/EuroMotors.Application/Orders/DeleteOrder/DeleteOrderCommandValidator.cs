using FluentValidation;

namespace EuroMotors.Application.Orders.DeleteOrder;

internal sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty();
    }
}
