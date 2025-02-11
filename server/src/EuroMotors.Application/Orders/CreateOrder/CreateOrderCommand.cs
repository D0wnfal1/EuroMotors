using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.CreateOrder;

public sealed record CreateOrderCommand(Guid CustomerId) : ICommand;
