using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCart;

public sealed record GetCartQuery(Guid UserId) : IQuery<Cart>;
