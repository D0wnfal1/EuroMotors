using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.AddItemToCart;

public sealed record AddItemToCartCommand(Guid CartId, Guid ProductId, int Quantity)
    : ICommand;
