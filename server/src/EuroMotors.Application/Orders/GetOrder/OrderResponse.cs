using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrder;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalPrice,
    DateTime CreatedAtUtc)
{
    public List<OrderItemResponse> OrderItems { get; } = [];
}
