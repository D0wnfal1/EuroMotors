using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IQueryHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = dbConnectionFactory.CreateConnection();

        const string sql =
            $"""
             SELECT
                 o.id AS {nameof(OrderResponse.Id)},
                 o.user_id AS {nameof(OrderResponse.UserId)},
                 o.status AS {nameof(OrderResponse.Status)},
                 o.total_price AS {nameof(OrderResponse.TotalPrice)},
                 o.delivery_method AS {nameof(OrderResponse.DeliveryMethod)},
                 o.shipping_address AS {nameof(OrderResponse.ShippingAddress)},
                 o.created_at_utc AS {nameof(OrderResponse.CreatedAtUtc)},
                 o.updated_at_utc AS {nameof(OrderResponse.UpdatedAtUtc)},
                 oi.id AS {nameof(OrderItemResponse.OrderItemId)},
                 oi.order_id AS {nameof(OrderItemResponse.OrderId)},
                 oi.product_id AS {nameof(OrderItemResponse.ProductId)},
                 oi.quantity AS {nameof(OrderItemResponse.Quantity)},
                 oi.unit_price AS {nameof(OrderItemResponse.UnitPrice)},
                 oi.price AS {nameof(OrderItemResponse.Price)}
             FROM orders o
             JOIN order_items oi ON oi.order_id = o.id
             WHERE o.id = @OrderId
             """;

        Dictionary<Guid, OrderResponse> ordersDictionary = [];
        await connection.QueryAsync<OrderResponse, OrderItemResponse, OrderResponse>(
            sql,
            (order, orderItem) =>
            {
                if (ordersDictionary.TryGetValue(order.Id, out OrderResponse? existingEvent))
                {
                    order = existingEvent;
                }
                else
                {
                    ordersDictionary.Add(order.Id, order);
                }

                order.OrderItems.Add(orderItem);

                return order;
            },
            new { request.OrderId },
            splitOn: nameof(OrderItemResponse.OrderItemId));

        return !ordersDictionary.TryGetValue(request.OrderId, out OrderResponse orderResponse) ? Result.Failure<OrderResponse>(OrderErrors.NotFound(request.OrderId)) : orderResponse;
    }
}
