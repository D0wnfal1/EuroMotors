using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

internal sealed class UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCartItemQuantityCommand>
{

    public async Task<Result> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(CartErrors.Empty);
        }

        await cartRepository.UpdateCartItemQuantityAsync(cart.Id, request.ProductId, request.NewQuantity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
