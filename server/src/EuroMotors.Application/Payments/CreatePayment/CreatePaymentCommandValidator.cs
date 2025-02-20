using FluentValidation;

namespace EuroMotors.Application.Payments.CreatePayment;

internal sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(p => p.OrderId).NotEmpty();
    }
}
