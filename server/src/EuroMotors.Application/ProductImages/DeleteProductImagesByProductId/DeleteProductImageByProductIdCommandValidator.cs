using FluentValidation;

namespace EuroMotors.Application.ProductImages.DeleteProductImagesByProductId;

internal sealed class DeleteProductImageByProductIdCommandValidator : AbstractValidator<DeleteProductImageByProductIdCommand>
{
    public DeleteProductImageByProductIdCommandValidator()
    {
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
