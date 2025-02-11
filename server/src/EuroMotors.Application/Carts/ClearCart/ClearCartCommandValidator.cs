using FluentValidation;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}
