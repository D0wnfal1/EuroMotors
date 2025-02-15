namespace EuroMotors.Application.Orders.GetOrderById;

public sealed record OrderItemResponse(
    Guid OrderItemId,
    Guid OrderId,
    Guid ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal Price);
