using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(CartErrors.Empty);
        }

        await cartRepository.ClearCartAsync(cart.Id, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
