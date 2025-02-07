using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public class ProductIsNotAvailableDomainEvent(Guid productId) : IDomainEvent
{
    public Guid ProductId { get; init; } = productId;
}
