using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandHandler(
    IProductRepository productRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        Cart? cart = null;

        if (request.UserId.HasValue)
        {
            cart = await cartRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
        }
        else if (request.SessionId.HasValue)
        {
            cart = await cartRepository.GetBySessionIdAsync(request.SessionId.Value, cancellationToken);
        }

        if (cart == null)
        {
            cart = request.UserId.HasValue
                ? Cart.CreateForUser(request.UserId.Value)
                : Cart.CreateForSession(request.SessionId!.Value);

            cartRepository.Insert(cart);
        }

        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(request.ProductId));
        }

        if (product.Stock < request.Quantity)
        {
            return Result.Failure(ProductErrors.NotEnoughStock(product.Stock));
        }

        var cartItem = CartItem.Create(product, cart.Id, request.Quantity);

        await cartRepository.AddItemToCartAsync(cartItem, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
