using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.ConvertToOrder;

public record ConvertToOrderCommand(Guid UserId) : ICommand;
