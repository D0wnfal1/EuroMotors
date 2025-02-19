using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Orders.GetOrders;

namespace EuroMotors.Application.Orders.GetUserOrders;

public sealed record GetUserOrdersQuery(Guid UserId) : IQuery<IReadOnlyCollection<OrdersResponse>>;
