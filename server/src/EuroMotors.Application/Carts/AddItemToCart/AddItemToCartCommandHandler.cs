using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandHandler(
    IProductRepository productRepository,
    CartService cartService) : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        Cart cart = await cartService.GetAsync(request.CartId, cancellationToken);

        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        if (product.Stock < request.Quantity)
        {
            return Result.Failure(ProductErrors.NotEnoughStock(product.Stock));
        }

        var cartItem = new CartItem
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = product.Price
        };

        await cartService.AddItemAsync(cart.Id, cartItem, cancellationToken);

        return Result.Success();
    }
}
