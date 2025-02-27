namespace EuroMotors.Application.Carts;

public sealed class Cart
{
    public Guid UserId { get; init; }

    public List<CartItem> CartItems { get; init; } = [];

    public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);

    internal static Cart CreateDefault(Guid customerId) => new() { UserId = customerId };
}
