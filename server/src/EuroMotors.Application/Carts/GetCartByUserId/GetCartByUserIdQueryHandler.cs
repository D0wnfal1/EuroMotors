using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCartByUserId;

internal sealed class GetCartByUserIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCartByUserIdQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 c.id AS {nameof(CartResponse.Id)},
                 c.user_id AS {nameof(CartResponse.UserId)},
                 c.session_id AS {nameof(CartResponse.SessionId)},
                 ci.id AS {nameof(CartItemResponse.CartItemId)},
                 ci.cart_id AS {nameof(CartItemResponse.CartId)},
                 ci.product_id AS {nameof(CartItemResponse.ProductId)},
                 ci.quantity AS {nameof(CartItemResponse.Quantity)},
                 ci.unit_price AS {nameof(CartItemResponse.UnitPrice)}
             FROM carts c
             LEFT JOIN cart_items ci ON ci.cart_id = c.id
             WHERE c.user_id = @UserId
             """;

        var cartsDictionary = new Dictionary<Guid, CartResponse>();

        await connection.QueryAsync<CartResponse, CartItemResponse, CartResponse>(
            sql,
            (cart, cartItem) =>
            {
                if (!cartsDictionary.TryGetValue(cart.UserId.GetValueOrDefault(), out CartResponse? existingCart))
                {
                    existingCart = new CartResponse(cart.Id, cart.UserId, null);
                    cartsDictionary[cart.UserId.GetValueOrDefault()] = existingCart;
                }

                existingCart.CartItems.Add(cartItem);

                return existingCart;
            },
            new
            {
                request.UserId
            },
            splitOn: nameof(CartItemResponse.CartItemId));

        return cartsDictionary.TryGetValue(request.UserId, out CartResponse? cartResponse)
            ? cartResponse
            : Result.Failure<CartResponse>(CartErrors.MissingIdentifiers);
    }
}
