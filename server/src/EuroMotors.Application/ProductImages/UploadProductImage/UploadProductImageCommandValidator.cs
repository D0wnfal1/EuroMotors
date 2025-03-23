using FluentValidation;

namespace EuroMotors.Application.ProductImages.UploadProductImage;

internal sealed class UploadProductImageCommandValidator : AbstractValidator<UploadProductImageCommand>
{
    public UploadProductImageCommandValidator()
    {
        RuleFor(c => c.File).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
    }
}
