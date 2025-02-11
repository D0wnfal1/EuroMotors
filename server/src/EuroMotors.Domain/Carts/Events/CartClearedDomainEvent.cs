using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts.Events;

public sealed class CartClearedDomainEvent(Guid cartId) : IDomainEvent
{
    public Guid CartId { get; init; } = cartId;
}
