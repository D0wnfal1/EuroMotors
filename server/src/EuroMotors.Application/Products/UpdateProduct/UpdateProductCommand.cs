using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(Guid ProductId, string Name, string Description, string VendorCode, Guid CategoryId, Guid CarModelId,
    decimal Price, decimal Discount,
    int Stock, bool IsAvailable) : ICommand;
