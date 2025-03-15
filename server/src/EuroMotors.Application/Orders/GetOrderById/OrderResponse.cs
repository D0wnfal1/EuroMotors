using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrderById;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalPrice,
    DeliveryMethod DeliveryMethod,
    string ShippingAddress,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc)
{
    public List<OrderItemResponse> OrderItems { get; } = [];
}
