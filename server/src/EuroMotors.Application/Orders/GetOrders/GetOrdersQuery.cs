using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.GetOrders;

public sealed record GetOrdersQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<OrderResponse>>;
