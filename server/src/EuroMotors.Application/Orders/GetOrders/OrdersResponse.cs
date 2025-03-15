using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed record OrdersResponse(
    Guid Id,
    Guid UserId,
    OrderStatus Status,
    decimal TotalPrice,
    DeliveryMethod DeliveryMethod,
    string ShippingAddress,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
