using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.CreateProduct;

public sealed record CreateProductCommand(string Name, string Description, string VendorCode,
    Guid CategoryId, Guid CarModelId,
    decimal Price, decimal Discount,
    int Stock, bool IsAvailable) : ICommand<Guid>;
