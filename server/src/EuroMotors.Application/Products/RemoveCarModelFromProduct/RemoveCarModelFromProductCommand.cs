using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.RemoveCarModelFromProduct;

public sealed record RemoveCarModelFromProductCommand(Guid ProductId, Guid CarModelId) : ICommand; 