using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Carts;

public static class CartErrors
{

    public static Error QuantityMustBeGreaterThanZero =>
        Error.Failure("Orders.QuantityMustBeGreaterThanZero", "Quantity must be greater than zero.");

    public static readonly Error Empty = Error.Problem("Carts.Empty", "The cart is empty");
}
