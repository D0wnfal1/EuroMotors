using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductPrice;

internal sealed class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Price).GreaterThan(0);
    }
}
