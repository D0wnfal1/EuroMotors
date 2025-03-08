using System.Text.Json.Serialization;

namespace EuroMotors.Application.Carts;

public sealed class Cart
{
    [JsonConstructor]
    private Cart(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }

    public List<CartItem> CartItems { get; init; } = [];

    public decimal TotalPrice => CartItems.Sum(item => item.TotalPrice);

    internal static Cart CreateDefault(Guid cartId) => new (cartId);
}
