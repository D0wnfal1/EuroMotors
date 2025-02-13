using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts.GetCartById;

namespace EuroMotors.Application.Carts.GetCartByUserId;

public sealed record GetCartByUserIdQuery(Guid UserId) : IQuery<CartResponse>;
