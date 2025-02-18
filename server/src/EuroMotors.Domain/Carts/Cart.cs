using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts.Events;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Domain.Carts;

public sealed class Cart : Entity
{
    private readonly List<CartItem> _cartItems = [];

    private Cart()
    {
    }

    public Guid UserId { get; private set; }

    public List<CartItem> CartItems { get; private set; } = [];

    public decimal TotalPrice => _cartItems.Sum(item => item.TotalPrice);

    public static Cart Create(Guid userId)
    {
        var cart = new Cart()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
        };

        cart.RaiseDomainEvent(new CartCreatedDomainEvent(cart.Id));

        return cart;
    }

    public void Clear()
    {
        _cartItems.Clear();
        RaiseDomainEvent(new CartClearedDomainEvent(Id));
    }

    public Order ConvertToOrder()
    {
        var order = Order.Create(UserId);

        foreach (CartItem cartItem in _cartItems)
        {
            order.AddItem(cartItem.ProductId, cartItem.Quantity, cartItem.UnitPrice);
        }

        return order;
    }
}
