using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Carts.UpdateItemQuantity;

internal sealed class UpdateItemQuantityCommandHandler(IProductRepository productRepository, CartService cartService)
    : ICommandHandler<UpdateItemQuantityCommand>
{
    public async Task<Result> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        await cartService.UpdateQuantityAsync(request.CartId, product.Id, request.Quantity, cancellationToken);

        return Result.Success();
    }
}
