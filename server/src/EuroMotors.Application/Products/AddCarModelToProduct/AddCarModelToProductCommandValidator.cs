using FluentValidation;

namespace EuroMotors.Application.Products.AddCarModelToProduct;

internal sealed class AddCarModelToProductCommandValidator : AbstractValidator<AddCarModelToProductCommand>
{
    public AddCarModelToProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .Must(x => x != Guid.Empty)
            .WithMessage("Product ID cannot be empty");

        RuleFor(x => x.CarModelId)
            .NotEmpty()
            .Must(x => x != Guid.Empty)
            .WithMessage("Car model ID cannot be empty");
    }
} 