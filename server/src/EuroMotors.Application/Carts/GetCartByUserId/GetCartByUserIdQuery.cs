using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.GetCartByUserId;

public sealed record GetCartByUserIdQuery(Guid UserId) : IQuery<Cart>;
