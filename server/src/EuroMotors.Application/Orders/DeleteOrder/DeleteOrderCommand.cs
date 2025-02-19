using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Orders.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId) : ICommand;
