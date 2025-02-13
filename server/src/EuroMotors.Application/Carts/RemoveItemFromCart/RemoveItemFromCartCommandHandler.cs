using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(
	IUserRepository userRepository,
	IProductRepository productRepository,
	ICartRepository cartRepository,
	IUnitOfWork unitOfWork)
	: ICommandHandler<RemoveItemFromCartCommand>
{
	public async Task<Result> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
	{
		User? customer = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

		if (customer is null)
		{
			return Result.Failure(UserErrors.NotFound(request.UserId));
		}

		Product? product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

		if (product is null)
		{
			return Result.Failure(ProductErrors.NotFound(request.ProductId));
		}
		Cart cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);

		if (cart is null)
		{
			return Result.Failure(CartErrors.NotFound(request.UserId));
		}

		CartItem? cartItem = cart.CartItems?.Find(c => c.ProductId == product.Id);

		if (cartItem is null)
		{
			return Result.Failure(CartErrors.Empty);
		}

		cart.RemoveItem(cartItem.ProductId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
