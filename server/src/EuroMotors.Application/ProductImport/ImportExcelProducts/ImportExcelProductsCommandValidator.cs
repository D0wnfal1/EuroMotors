using FluentValidation;

namespace EuroMotors.Application.ProductImport.ImportExcelProducts;

public sealed class ImportExcelProductsCommandValidator : AbstractValidator<ImportExcelProductsCommand>
{
    public ImportExcelProductsCommandValidator()
    {
        RuleFor(x => x.PriceFileStream)
            .NotNull()
            .WithMessage("Price file stream cannot be null");

    }
} 
