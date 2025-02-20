using FluentValidation;

namespace EuroMotors.Application.Orders.SetPayment;

internal sealed class SetPaymentCommandValidator : AbstractValidator<SetPaymentCommand>
{
    public SetPaymentCommandValidator()
    {
        RuleFor(p => p.OrderId).NotEmpty();
        RuleFor(p => p.PaymentId).NotEmpty();
        RuleFor(p => p.PaymentStatus).IsInEnum();
    }
}
