using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts.Events;

public sealed class CartItemUpdatedDomainEvent(Guid cartId, Guid productId) : IDomainEvent
{
    public Guid CartId { get; init; } = cartId;
    public Guid ProductId { get; init; } = productId;
}
