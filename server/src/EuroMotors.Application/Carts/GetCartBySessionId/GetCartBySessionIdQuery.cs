using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.GetCartBySessionId;

public sealed record GetCartBySessionIdQuery(Guid SessionId) : IQuery<CartResponse>;
