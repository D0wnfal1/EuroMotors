using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

internal sealed class UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateCartItemQuantityCommand>
{

    public async Task<Result> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        Cart? cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (cart == null)
        {
            return Result.Failure(CartErrors.NotFound(request.UserId));
        }

        await cartRepository.UpdateCartItemQuantityAsync(request.UserId, request.ProductId, request.NewQuantity, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
