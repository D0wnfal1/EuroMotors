using FluentValidation;

namespace EuroMotors.Application.Products.DeleteProduct;

internal sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
