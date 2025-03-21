using FluentValidation;

namespace EuroMotors.Application.ProductImages.UpdateProductImage;

internal sealed class UpdateProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
{
    public UpdateProductImageCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.File).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
