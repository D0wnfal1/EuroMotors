using FluentValidation;

namespace EuroMotors.Application.Products.CopyProduct;

internal sealed class CopyProductCommandValidator : AbstractValidator<CopyProductCommand>
{
    public CopyProductCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
