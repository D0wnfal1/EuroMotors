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

    public Guid? UserId { get; private set; }
    public Guid? SessionId { get; private set; }

    public List<CartItem> CartItems { get; private set; } = [];

    public decimal TotalPrice => _cartItems.Sum(item => item.TotalPrice);

    public static Cart CreateForUser(Guid userId)
    {
        var cart = new Cart()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
        };

        cart.RaiseDomainEvent(new CartCreatedDomainEvent(cart.Id));

        return cart;
    }

    public static Cart CreateForSession(Guid userId)
    {
        var cart = new Cart()
        {
            Id = Guid.NewGuid(),
            SessionId = userId,
        };

        cart.RaiseDomainEvent(new CartCreatedDomainEvent(cart.Id));

        return cart;
    }
}
