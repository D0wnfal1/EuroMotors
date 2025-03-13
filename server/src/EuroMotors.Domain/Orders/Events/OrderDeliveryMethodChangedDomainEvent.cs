using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Orders.Events;

public sealed class OrderDeliveryMethodChangedDomainEvent(
    Guid id,
    DeliveryMethod deliveryMethod,
    string? deliveryDetails) : IDomainEvent
{
    public Guid Id { get; set; } = id;
    public DeliveryMethod DeliveryMethod { get; set; } = deliveryMethod;
    public string? DeliveryDetails { get; set; } = deliveryDetails;
}
