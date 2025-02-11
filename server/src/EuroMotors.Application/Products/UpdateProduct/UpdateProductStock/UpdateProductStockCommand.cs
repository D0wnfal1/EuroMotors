using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProduct.UpdateProductStock;

public record UpdateProductStockCommand(Guid ProductId, int Quantity) : ICommand;
