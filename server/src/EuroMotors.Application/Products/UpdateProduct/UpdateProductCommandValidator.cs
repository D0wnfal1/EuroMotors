using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().Must(x => x != Guid.Empty);

        RuleFor(x => x.Name).NotEmpty().Length(3, 100);

        RuleFor(x => x.VendorCode).NotEmpty().Length(3, 50);

        RuleFor(x => x.CategoryId).NotEmpty().Must(x => x != Guid.Empty);

        RuleFor(x => x.Price).GreaterThan(0);

        RuleFor(x => x.Discount).InclusiveBetween(0, 100);

        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
