using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Carts.GetCartByUserId;

internal sealed class GetCartByUserIdQueryHandler(CartService cartService)
    : IQueryHandler<GetCartByUserIdQuery, Cart>
{
    public async Task<Result<Cart>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await cartService.GetAsync(request.UserId, cancellationToken);
    }
}


