using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts.Events;

public sealed class CartCreatedDomainEvent(Guid cartId) : IDomainEvent
{
    public Guid CartId { get; init; } = cartId;
}
