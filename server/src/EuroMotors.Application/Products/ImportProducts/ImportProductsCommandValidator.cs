using FluentValidation;

namespace EuroMotors.Application.Products.ImportProducts;

public sealed class ImportProductsCommandValidator : AbstractValidator<ImportProductsCommand>
{
    public ImportProductsCommandValidator()
    {
        RuleFor(x => x.FileStream)
            .NotNull()
            .WithMessage("File stream cannot be null");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name cannot be empty")
            .Must(fileName => fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("File must be a CSV file");
    }
}