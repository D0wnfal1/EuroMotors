using System.Data.Common;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetOrdersQuery, IReadOnlyCollection<OrderResponse>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(OrderResponse.Id)},
                 user_id AS {nameof(OrderResponse.UserId)},
                 status AS {nameof(OrderResponse.Status)},
                 total_price AS {nameof(OrderResponse.TotalPrice)},
                 created_at_utc AS {nameof(OrderResponse.CreatedAtUtc)}
             FROM orders
             WHERE user_id = @UserId
             """;

        List<OrderResponse> orders = (await connection.QueryAsync<OrderResponse>(sql, request)).AsList();

        return orders;
    }
}
