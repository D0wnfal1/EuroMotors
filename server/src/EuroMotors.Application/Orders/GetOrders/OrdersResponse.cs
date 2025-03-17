using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed class OrdersResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public OrderStatus Status { get; init; }
    public decimal TotalPrice { get; init; }
    public DeliveryMethod DeliveryMethod { get; init; }
    public string ShippingAddress { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }

    public OrdersResponse()
    {

    }
}

