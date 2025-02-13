using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCartById;

internal sealed class GetCartByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCartByIdQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
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
             WHERE c.id = @CartId
             """;

        Dictionary<Guid, CartResponse> cartsDictionary = [];

        await connection.QueryAsync<CartResponse, CartItemResponse, CartResponse>(
            sql,
            (cart, cartItem) =>
            {
                if (!cartsDictionary.TryGetValue(cart.Id, out CartResponse? existingCart))
                {
                    existingCart = cart;
                    cartsDictionary[cart.Id] = existingCart;
                }

                if (cartItem is not null)
                {
                    existingCart.CartItems.Add(cartItem);
                }

                return existingCart;
            },
            new { request.CartId },
            splitOn: nameof(CartItemResponse.CartItemId));

        return cartsDictionary.TryGetValue(request.CartId, out CartResponse? cartResponse)
            ? cartResponse
            : Result.Failure<CartResponse>(CartErrors.NotFound(request.CartId));
    }
}
