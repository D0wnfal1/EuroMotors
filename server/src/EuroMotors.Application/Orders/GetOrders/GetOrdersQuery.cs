using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed record GetOrdersQuery() : IQuery<IReadOnlyCollection<OrdersResponse>>;
