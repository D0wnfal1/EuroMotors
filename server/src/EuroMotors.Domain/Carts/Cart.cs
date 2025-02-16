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

    public Result AddItem(CartItem cartItem)
    {
        if (cartItem.Quantity <= 0)
        {
            return Result.Failure(CartErrors.QuantityMustBeGreaterThanZero);
        }

        CartItem? existingItem = _cartItems.FirstOrDefault(item => item.ProductId == cartItem.ProductId);

        if (existingItem != null)
        {
            Result updateResult = existingItem.UpdateQuantity(existingItem.Quantity + cartItem.Quantity);
            if (updateResult.IsFailure)
            {
                return updateResult;
            }
        }
        else
        {
            _cartItems.Add(cartItem);
        }

        RaiseDomainEvent(new CartItemAddedDomainEvent(Id, cartItem.ProductId));

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
        var order = Order.Create(UserId);

        foreach (CartItem cartItem in _cartItems)
        {
            order.AddItem(cartItem.ProductId, cartItem.Quantity, cartItem.UnitPrice);
        }

        Clear();

        return order;
    }
}
