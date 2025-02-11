using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(Guid ProductId, decimal Price) : ICommand;
