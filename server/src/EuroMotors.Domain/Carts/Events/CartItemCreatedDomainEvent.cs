using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts.Events;

public sealed class CartItemCreatedDomainEvent(Guid cartItemId) : IDomainEvent
{
    public Guid CartItemId { get; init; } = cartItemId;
}
