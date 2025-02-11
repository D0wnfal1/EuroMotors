using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalPrice,
    DateTime CreatedAtUtc);
