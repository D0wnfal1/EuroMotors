using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(
	IProductRepository productRepository,
	ICartRepository cartRepository,
	IUnitOfWork unitOfWork) : ICommandHandler<RemoveItemFromCartCommand>
{
	public async Task<Result> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
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

        if (cart is null)
        {
            return Result.Failure(CartErrors.MissingIdentifiers);
        }

        Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

		if (product is null)
		{
			return Result.Failure(ProductErrors.NotFound(request.ProductId));
		}

		CartItem? cartItem = cart.CartItems?.Find(c => c.ProductId == product.Id);

		if (cartItem is null)
		{
			return Result.Failure(CartErrors.Empty);
		}

		await cartRepository.RemoveItemFromCartAsync(cartItem);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
	}
}
