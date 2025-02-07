using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Orders.Events;

public sealed class OrderCreatedDomainEvent(Guid orderId) : IDomainEvent
{
    public Guid OrderId { get; init; } = orderId;
}
