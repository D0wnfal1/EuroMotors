using FluentValidation;

namespace EuroMotors.Application.ProductImages.DeleteProductImage;

internal sealed class DeleteProductImageCommandValidator : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
