using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Carts.ConvertToOrder;

internal sealed class ConvertToOrderCommandHandler(ICartRepository cartRepository, IProductRepository productRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ConvertToOrderCommand>
{
    public async Task<Result> Handle(ConvertToOrderCommand request, CancellationToken cancellationToken)
    {
        Cart? cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (cart == null || !cart.CartItems.Any())
        {
            return Result.Failure(CartErrors.NotFound(request.UserId));
        }

        var order = Order.Create(cart.UserId, cart.CartItems);

        foreach (CartItem cartItem in cart.CartItems)
        {
            Product? product = await productRepository.GetByIdAsync(cartItem.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Failure(ProductErrors.NotFound(cartItem.ProductId));
            }

            Result result = product.SubtractProductQuantity(cartItem.Quantity);

            if (result.IsFailure)
            {
                return result;
            }
        }

        orderRepository.Insert(order);

        await orderRepository.AddItemsToOrder(order, cancellationToken);

        await cartRepository.ClearCartAsync(cart.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
