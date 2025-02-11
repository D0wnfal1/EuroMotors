using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts.Events;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.Carts;

public sealed class Cart : Entity
{
    private readonly List<CartItem> _cartItems = [];

    private Cart()
    {
    }

    public Guid UserId { get; private set; }

    public User User { get; private set; }

    public IReadOnlyCollection<CartItem> CartItems => [];

    public decimal TotalPrice => _cartItems.Sum(item => item.TotalPrice);

    public static Cart Create(User user)
    {
        var cart = new Cart()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user
        };

        cart.RaiseDomainEvent(new CartCreatedDomainEvent(cart.Id));

        return cart;
    }

    public Result AddItem(Product product, int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(CartErrors.QuantityMustBeGreaterThanZero);
        }

        CartItem? existingItem = _cartItems.FirstOrDefault(item => item.ProductId == product.Id);

        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var cartItem = CartItem.Create(product, quantity);
            _cartItems.Add(cartItem);
        }

        RaiseDomainEvent(new CartItemAddedDomainEvent(Id, product.Id));

        return Result.Success();
    }

    public Result RemoveItem(Guid productId)
    {
        CartItem? item = _cartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
        {
            return Result.Failure(CartErrors.ItemNotFound(productId));
        }

        _cartItems.Remove(item);
        RaiseDomainEvent(new CartItemRemovedDomainEvent(Id, productId));

        return Result.Success();
    }

    public Result UpdateItemQuantity(Guid productId, int newQuantity)
    {
        if (newQuantity <= 0)
        {
            return Result.Failure(CartErrors.QuantityMustBeGreaterThanZero);
        }

        CartItem? item = _cartItems.FirstOrDefault(x => x.ProductId == productId);

        if (item == null)
        {
            return Result.Failure(CartErrors.ItemNotFound(productId));
        }

        item.UpdateQuantity(newQuantity);
        RaiseDomainEvent(new CartItemUpdatedDomainEvent(Id, productId));

        return Result.Success();
    }

    public void Clear()
    {
        _cartItems.Clear();
        RaiseDomainEvent(new CartClearedDomainEvent(Id));
    }

    public Order ConvertToOrder()
    {
        var order = Order.Create(User);

        foreach (CartItem? cartItem in _cartItems)
        {
            order.AddItem(cartItem.Product, cartItem.Quantity, cartItem.Product.Price);
        }

        Clear();

        return order;
    }
}
