using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandHandler(IUserRepository userRepository, CartService cartService)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        User? customer = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (customer is null)
        {
            return Result.Failure(UserErrors.NotFound(request.UserId));
        }

        await cartService.ClearAsync(customer.Id, cancellationToken);

        return Result.Success();
    }
}
