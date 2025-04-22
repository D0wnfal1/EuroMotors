using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.AddCarModelToProduct;

public sealed record AddCarModelToProductCommand(Guid ProductId, Guid CarModelId) : ICommand; 