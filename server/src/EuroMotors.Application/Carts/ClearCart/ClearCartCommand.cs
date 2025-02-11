using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.ClearCart;

public sealed record ClearCartCommand(Guid UserId) : ICommand;
