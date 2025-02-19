using FluentValidation;

namespace EuroMotors.Application.Orders.DeleteOrder;

public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(o => o.OrderId).NotEmpty();
    }
}
