using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandHandler(IUserRepository userRepository, ICartRepository cartRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        Cart cart = await cartRepository.GetByUserIdAsync(user.Id, cancellationToken) ?? Cart.Create(user.Id);

        cart.Clear();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
