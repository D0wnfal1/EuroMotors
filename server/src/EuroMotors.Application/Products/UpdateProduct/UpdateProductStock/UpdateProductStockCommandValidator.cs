using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductStock;

internal sealed class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
