using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.UpdateItemQuantity;

internal sealed class UpdateItemQuantityCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateItemQuantityCommand>
{

    public async Task<Result> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
    {
        Cart? cart = await cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (cart == null)
        {
            return Result.Failure(CartErrors.NotFound(request.UserId));
        }

        Result result = cart.UpdateItemQuantity(request.ProductId, request.NewQuantity);

        if (result.IsFailure)
        {
            return result;
        }

        cartRepository.Update(cart);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
