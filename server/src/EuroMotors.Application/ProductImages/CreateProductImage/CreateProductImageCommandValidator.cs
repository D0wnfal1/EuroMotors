using FluentValidation;

namespace EuroMotors.Application.ProductImages.CreateProductImage;

internal sealed class CreateProductImageCommandValidator : AbstractValidator<CreateProductImageCommand>
{
    public CreateProductImageCommandValidator()
    {
        RuleFor(c => c.Url).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
