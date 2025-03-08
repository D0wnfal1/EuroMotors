using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.UpdateItemQuantity;

public sealed record UpdateItemQuantityCommand(Guid CartId, Guid ProductId, int Quantity) : ICommand;
