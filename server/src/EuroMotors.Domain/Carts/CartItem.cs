using EuroMotors.Domain.Abstractions;
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

        return cartItem;
    }

    public Result UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(CartErrors.QuantityMustBeGreaterThanZero);
        }

        Quantity = quantity;

        return Result.Success();
    }
}
