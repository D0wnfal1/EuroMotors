using FluentValidation;

namespace EuroMotors.Application.Products.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name cannot be empty.")
            .Length(3, 100).WithMessage("Product name must be between 3 and 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description cannot be empty.")
            .Length(10, 500).WithMessage("Product description must be between 10 and 500 characters.");

        RuleFor(x => x.VendorCode)
            .NotEmpty().WithMessage("Product vendor code cannot be empty.")
            .Length(3, 50).WithMessage("Product vendor code must be between 3 and 50 characters.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID cannot be empty.")
            .Must(x => x != Guid.Empty).WithMessage("Category ID must be a valid GUID.");

        RuleFor(x => x.CarModelId)
            .NotEmpty().WithMessage("Car model ID cannot be empty.")
            .Must(x => x != Guid.Empty).WithMessage("Car model ID must be a valid GUID.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative.");

        RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Product availability must be true or false.");
    }
}

