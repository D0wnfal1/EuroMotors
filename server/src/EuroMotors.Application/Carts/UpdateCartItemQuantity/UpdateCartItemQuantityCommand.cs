using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

public record UpdateCartItemQuantityCommand(Guid UserId, Guid ProductId, int NewQuantity) : ICommand;
