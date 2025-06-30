using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImport.ImportExcelProducts;

public sealed record ImportExcelProductsCommand(
    Stream PriceFileStream,
    Stream? MappingFileStream) : ICommand<ImportExcelProductsResult>;

public sealed record ImportExcelProductsResult(
    int TotalProcessed,
    int SuccessfullyImported,
    int Failed,
    List<string> Errors); 