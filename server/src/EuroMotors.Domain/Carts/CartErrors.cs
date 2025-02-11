using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts;

public static class CartErrors
{
    public static Error NotFound(Guid cartId) =>
        Error.NotFound("Carts.NotFound", $"The cart with the identifier {cartId} was not found.");

    public static Error ItemNotFound(Guid productId) =>
        Error.NotFound("Carts.ItemNotFound", $"The item with product ID {productId} was not found in the cart.");

    public static Error NotEnoughStock(int stock) =>
        Error.Failure("Carts.NotEnoughStock", $"Not enough stock available. Only {stock} items left.");
    public static Error QuantityMustBeGreaterThanZero =>
        Error.Failure("Orders.QuantityMustBeGreaterThanZero", "Quantity must be greater than zero.");
}
