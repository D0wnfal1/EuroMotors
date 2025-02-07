using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Orders;

public static class OrderErrors
{
    public static Error NotFound(Guid orderId) =>
        Error.NotFound("Product.NotFound", $"The order with the identifier {orderId} was not found");


    public static readonly Error ProductIsNotAvailable= Error.Problem(
        "Product.ProductIsNotAvailable",
        "The product for this order were already issued");
}
