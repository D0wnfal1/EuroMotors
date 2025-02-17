using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProduct;

internal sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description).MaximumLength(1000);

        RuleFor(x => x.VendorCode).NotEmpty();

        RuleFor(x => x.CategoryId).NotEmpty();

        RuleFor(x => x.CarModelId).NotEmpty();

        RuleFor(x => x.Price).GreaterThan(0);

        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);

        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}
