using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderResponse>;
