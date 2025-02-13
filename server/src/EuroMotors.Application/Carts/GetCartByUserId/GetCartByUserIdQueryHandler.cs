using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCartByUserId;

internal sealed class GetCartByUserIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCartByUserIdQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 c.id AS {nameof(CartResponse.Id)},
                 c.user_id AS {nameof(CartResponse.UserId)},
                 ci.id AS {nameof(CartItemResponse.CartItemId)},
                 ci.cart_id AS {nameof(CartItemResponse.CartId)},
                 ci.product_id AS {nameof(CartItemResponse.ProductId)},
                 ci.quantity AS {nameof(CartItemResponse.Quantity)},
                 ci.unit_price AS {nameof(CartItemResponse.UnitPrice)},
                 ci.total_price AS {nameof(CartItemResponse.TotalPrice)}
             FROM carts c
             LEFT JOIN cart_items ci ON ci.cart_id = c.id
             WHERE c.user_id = @UserId
             """;

        Dictionary<Guid, CartResponse> cartsDictionary = [];
        await connection.QueryAsync<CartResponse, CartItemResponse, CartResponse>(
            sql,
            (cart, cartItem) =>
            {
                if (cartsDictionary.TryGetValue(cart.Id, out CartResponse? existingCart))
                {
                    cart = existingCart;
                }
                else
                {
                    cartsDictionary.Add(cart.Id, cart);
                }

                if (cartItem is not null)
                {
                    cart.CartItems.Add(cartItem);
                }

                return cart;
            },
            request,
            splitOn: nameof(CartItemResponse.CartItemId));

        return !cartsDictionary.TryGetValue(request.UserId, out CartResponse cartResponse) ? Result.Failure<CartResponse>(CartErrors.NotFound(request.UserId)) : cartResponse;
    }
}
