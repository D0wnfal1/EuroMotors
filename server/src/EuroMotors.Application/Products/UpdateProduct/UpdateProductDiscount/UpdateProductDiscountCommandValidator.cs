using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductDiscount;

internal sealed class UpdateProductDiscountCommandValidator : AbstractValidator<UpdateProductDiscountCommand>
{
    public UpdateProductDiscountCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
    }
}
