using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.GetOrder;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;
