using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Carts;

public class CartItem : Entity
{
    private CartItem()
    {
    }

    public Guid ProductId { get; private set; }

    public Guid CartId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal TotalPrice => UnitPrice * Quantity;

    public static CartItem Create(Product product, Guid cartId, int quantity)
    {
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            CartId = cartId,
            Quantity = quantity,
            UnitPrice = product.CalculateDiscountedPrice()

        };

        cartItem.RaiseDomainEvent(new CartCreatedDomainEvent(cartItem.Id));

        return cartItem;
    }

    public Result UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            return Result.Failure(CartErrors.QuantityMustBeGreaterThanZero);
        }

        Quantity = newQuantity;

        return Result.Success();
    }
}
