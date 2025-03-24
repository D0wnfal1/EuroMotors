using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrderById;

public sealed record OrderResponse(
    Guid Id,
    Guid UserId,
    string BuyerName,
    string BuyerPhoneNumber,
    string BuyerEmail,
    OrderStatus Status,
    decimal TotalPrice,
    DeliveryMethod DeliveryMethod,
    string ShippingAddress,
    PaymentMethod PaymentMethod,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc)
{
    public List<OrderItemResponse> OrderItems { get; } = [];
}
