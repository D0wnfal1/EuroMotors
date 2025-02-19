using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Orders;

namespace EuroMotors.Application.Orders.ChangeOrderStatus;

public sealed record ChangeOrderStatusCommand(Guid OrderId, OrderStatus Status) : ICommand;
