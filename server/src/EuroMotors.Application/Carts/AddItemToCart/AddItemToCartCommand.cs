using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.AddItemToCart;

public sealed record AddItemToCartCommand(Guid? UserId, Guid? SessionId, Guid ProductId, int Quantity)
    : ICommand;
