using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Carts;

public static class CartErrors
{
    public static Error NotFound(Guid cartId) =>
        Error.NotFound("Carts.NotFound", $"The cart with the identifier {cartId} was not found.");

    public static Error QuantityMustBeGreaterThanZero =>
        Error.Failure("Orders.QuantityMustBeGreaterThanZero", "Quantity must be greater than zero.");

    public static readonly Error Empty = Error.Problem("Carts.Empty", "The cart is empty");
}
