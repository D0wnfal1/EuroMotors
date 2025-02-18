using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCartTotalPrice;

internal sealed class GetCartTotalPriceQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetCartTotalPriceQuery, decimal>
{
    public async Task<Result<decimal>> Handle(GetCartTotalPriceQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql = 
            """
                SELECT COALESCE(SUM(ci.quantity * ci.unit_price), 0)
                FROM carts c
                LEFT JOIN cart_items ci ON ci.cart_id = c.id
                WHERE c.id = @CartId
                GROUP BY c.id
            """;

        decimal? totalPrice = await connection.ExecuteScalarAsync<decimal?>(sql, new { request.CartId });

        return totalPrice ?? Result.Failure<decimal>(CartErrors.NotFound(request.CartId));
    }
}
