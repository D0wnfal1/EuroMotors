using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Carts.GetCartById;

internal sealed class GetCartByIdQueryHandler(CartService cartService)
    : IQueryHandler<GetCartByIdQuery, Cart>
{
    public async Task<Result<Cart>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        return await cartService.GetAsync(request.CartId, cancellationToken);
    }
}


