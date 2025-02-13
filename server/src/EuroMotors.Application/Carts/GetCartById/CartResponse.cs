namespace EuroMotors.Application.Carts.GetCartById;

public sealed record CartResponse(
    Guid Id,
    Guid UserId)
{
    public List<CartItemResponse> CartItems { get; } = [];
}
