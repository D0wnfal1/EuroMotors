using System.Data;
using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Application.Orders.GetOrders;

internal sealed class GetOrdersQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>
{
    public async Task<Result<Pagination<OrdersResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine($"""
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
                        """);

        var parameters = new Dictionary<string, object>
        {
            { "PageSize", request.PageSize },
            { "Offset", (request.PageNumber - 1) * request.PageSize }
        };

        if (request.Status != null)
        {
            sql.AppendLine("WHERE status = @StatusText");
            parameters["StatusText"] = request.Status.ToString() ?? string.Empty;
        }

        sql.AppendLine("ORDER BY created_at_utc DESC");
        sql.AppendLine("LIMIT @PageSize OFFSET @Offset");

        List<OrdersResponse> orders = (await connection.QueryAsync<OrdersResponse>(sql.ToString(), parameters)).AsList();

        var countSql = new StringBuilder();
        countSql.Append("SELECT COUNT(*) FROM orders ");
        if (request.Status != null)
        {
            countSql.Append("WHERE status = @StatusText");
        }

        int totalCount = await connection.ExecuteScalarAsync<int>(countSql.ToString(), parameters);

        var paginatedResult = new Pagination<OrdersResponse>
        {
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
            Count = totalCount,
            Data = orders
        };

        return Result.Success(paginatedResult);
    }
}
