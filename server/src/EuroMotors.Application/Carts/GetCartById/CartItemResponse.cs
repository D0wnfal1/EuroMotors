namespace EuroMotors.Application.Carts.GetCartById;

public sealed record CartItemResponse(
    Guid CartItemId,
    Guid CartId,
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice);
