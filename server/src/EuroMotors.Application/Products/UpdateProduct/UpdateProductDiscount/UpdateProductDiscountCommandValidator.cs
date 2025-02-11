using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductDiscount;

public class UpdateProductDiscountCommandValidator : AbstractValidator<UpdateProductDiscountCommand>
{
    public UpdateProductDiscountCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId cannot be empty.");
        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to 0.")
            .LessThanOrEqualTo(100).WithMessage("Discount must be less than or equal to 100.");
    }
}
