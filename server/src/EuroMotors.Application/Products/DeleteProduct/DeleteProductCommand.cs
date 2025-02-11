using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : ICommand;

