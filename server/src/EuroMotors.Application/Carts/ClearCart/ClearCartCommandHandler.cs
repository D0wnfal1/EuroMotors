using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Carts.ClearCart;

internal sealed class ClearCartCommandHandler(CartService cartService)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        await cartService.ClearAsync(request.CartId, cancellationToken);

        return Result.Success();
    }
}
