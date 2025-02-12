using FluentValidation;

namespace EuroMotors.Application.Carts.ConvertToOrder;

public class ConvertToOrderCommandValidator : AbstractValidator<ConvertToOrderCommand>
{
    public ConvertToOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
