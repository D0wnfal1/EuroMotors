using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Carts.ConvertToOrder;

internal sealed class ConvertToOrderCommandHandler(ICartRepository cartRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ConvertToOrderCommand>
{
    public async Task<Result> Handle(ConvertToOrderCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(CartErrors.MissingIdentifiers);
        }

        if (!cart.CartItems.Any())
        {
            return Result.Failure(CartErrors.Empty);
        }

        var order = Order.Create(cart.UserId, cart.SessionId, cart.CartItems);

        orderRepository.Insert(order);

        await orderRepository.AddItemsToOrder(order, cancellationToken);

        await cartRepository.ClearCartAsync(cart.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
