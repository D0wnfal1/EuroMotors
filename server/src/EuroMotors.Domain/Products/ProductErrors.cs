using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products;

public static class ProductErrors
{
    public static Error NotFound(Guid productId) =>
        Error.NotFound("Product.NotFound", $"The product with the identifier {productId} was not found");

    public static Error NotEnoughStock(int stock) =>
        Error.Failure("Product.NotEnoughStock", $"Not enough stock available. Only {stock} items left.");
}
