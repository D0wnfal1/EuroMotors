using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProduct;

public sealed record UpdateProductCommand(Guid ProductId, string Name, string Description,
    decimal Price, decimal Discount,
    int Stock, Guid CategoryId, Guid CarModelId) : ICommand;
