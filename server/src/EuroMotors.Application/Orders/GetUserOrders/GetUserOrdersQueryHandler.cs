using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Orders.GetUserOrders;

internal sealed class GetUserOrdersQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetUserOrdersQuery, IReadOnlyCollection<OrdersResponse>>
{
    public async Task<Result<IReadOnlyCollection<OrdersResponse>>> Handle(
        GetUserOrdersQuery request,
        CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
              SELECT
                  id AS {nameof(OrdersResponse.Id)},
                  user_id AS {nameof(OrdersResponse.UserId)},
                  status AS {nameof(OrdersResponse.Status)},
                  total_price AS {nameof(OrdersResponse.TotalPrice)},
                  delivery_method AS {nameof(OrdersResponse.DeliveryMethod)},
                  shipping_address AS {nameof(OrdersResponse.ShippingAddress)},
                  created_at_utc AS {nameof(OrdersResponse.CreatedAtUtc)},
                  updated_at_utc AS {nameof(OrdersResponse.UpdatedAtUtc)}
              FROM orders
              WHERE user_id = @UserId
              """;

        List<OrdersResponse> orders = (await connection.QueryAsync<OrdersResponse>(sql, request)).AsList();

        return orders;
    }
}
