using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Carts;

public class CartItem : Entity
{
    private CartItem()
    {
    }

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; }

    public int Quantity { get; private set; }

    public decimal Price => Product.CalculateDiscountedPrice();

    public decimal TotalPrice => Price * Quantity;

    public static CartItem Create(Product product, int quantity)
    {
        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = quantity
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
