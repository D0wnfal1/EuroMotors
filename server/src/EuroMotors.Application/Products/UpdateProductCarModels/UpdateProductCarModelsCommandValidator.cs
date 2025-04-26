using FluentValidation;

namespace EuroMotors.Application.Products.UpdateProductCarModels;

internal sealed class UpdateProductCarModelsCommandValidator : AbstractValidator<UpdateProductCarModelsCommand>
{
    public UpdateProductCarModelsCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .Must(x => x != Guid.Empty)
            .WithMessage("Product ID cannot be empty");

        RuleFor(x => x.CarModelIds)
            .NotNull()
            .WithMessage("Car model IDs cannot be null");

        RuleForEach(x => x.CarModelIds)
            .NotEmpty()
            .Must(x => x != Guid.Empty)
            .WithMessage("Car model ID cannot be empty");
    }
}