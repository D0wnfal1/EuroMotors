using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.AddItemToCart;

public sealed record AddItemToCartCommand(Guid UserId, Guid ProductId, int Quantity)
    : ICommand;
