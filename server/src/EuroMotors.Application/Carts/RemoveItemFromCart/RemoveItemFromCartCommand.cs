using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

public sealed record RemoveItemFromCartCommand(Guid CartId, Guid ProductId) : ICommand;
