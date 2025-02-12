using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.UpdateItemQuantity;

public record UpdateItemQuantityCommand(Guid UserId, Guid ProductId, int NewQuantity) : ICommand;
