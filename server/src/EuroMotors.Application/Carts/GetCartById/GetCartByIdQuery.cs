using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.GetCartById;

public sealed record GetCartByIdQuery(Guid CartId) : IQuery<Cart>;
