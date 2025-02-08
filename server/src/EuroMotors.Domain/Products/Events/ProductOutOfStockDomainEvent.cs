using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed class ProductOutOfStockDomainEvent(Guid productId) : IDomainEvent
{
    public Guid ProductId { get; init; } = productId;
}
