using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts.Events;

public sealed class CartItemRemovedDomainEvent(Guid cartId, Guid productId) : IDomainEvent
{
    public Guid CartId { get; init; } = cartId;
    public Guid ProductId { get; init; } = productId;
}
