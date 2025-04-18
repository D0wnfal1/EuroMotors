using FluentValidation;

namespace EuroMotors.Application.Products.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().Length(3, 100);

        RuleFor(x => x.VendorCode).NotEmpty().Length(3, 50);

        RuleFor(x => x.CategoryId).NotEmpty().Must(x => x != Guid.Empty);

        RuleFor(x => x.CarModelId).NotEmpty().Must(x => x != Guid.Empty);

        RuleFor(x => x.Price).GreaterThan(0);

        RuleFor(x => x.Discount).InclusiveBetween(0, 100);

        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);

    }
}

