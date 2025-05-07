using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.ImportProducts;

public sealed record ImportProductsCommand(
    Stream FileStream,
    string FileName) : ICommand<ImportProductsResult>;

public sealed record ImportProductsResult(
    int TotalProcessed,
    int SuccessfullyImported,
    int Failed,
    List<string> Errors);
