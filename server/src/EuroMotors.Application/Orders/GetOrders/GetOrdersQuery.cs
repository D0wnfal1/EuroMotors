using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed record GetOrdersQuery(
    int PageNumber,
    int PageSize,
    OrderStatus? Status
) : IQuery<Pagination<OrdersResponse>>;
