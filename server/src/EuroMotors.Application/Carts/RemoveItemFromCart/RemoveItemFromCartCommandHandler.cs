using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(IProductRepository productRepository, CartService cartService)
    : ICommandHandler<RemoveItemFromCartCommand>
{
    public async Task<Result> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        await cartService.RemoveItemAsync(request.CartId, product.Id, cancellationToken);

        return Result.Success();
    }
}
