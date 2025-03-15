using EuroMotors.Domain.Orders;
using FluentValidation;

namespace EuroMotors.Application.Orders.CreateOrder;

internal sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(c => c.CartId).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty().When(c => c.UserId.HasValue);
        RuleFor(c => c.DeliveryMethod).NotEmpty();
        RuleFor(c => c.ShippingAddress)
            .NotEmpty()
            .When(c => c.DeliveryMethod == DeliveryMethod.Delivery);
        RuleFor(c => c.PaymentMethod).NotEmpty();
    }
}
