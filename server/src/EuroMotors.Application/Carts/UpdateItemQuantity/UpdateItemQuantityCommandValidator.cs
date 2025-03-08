using FluentValidation;

namespace EuroMotors.Application.Carts.UpdateItemQuantity;

internal sealed class UpdateItemQuantityCommandValidator : AbstractValidator<UpdateItemQuantityCommand>
{
    public UpdateItemQuantityCommandValidator()
    {
        RuleFor(c => c.CartId)
            .NotEmpty();
        RuleFor(c => c.Quantity).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
