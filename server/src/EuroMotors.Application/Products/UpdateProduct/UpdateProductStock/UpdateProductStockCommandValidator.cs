using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductStock;

internal sealed class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId cannot be empty.");
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
    }
}
