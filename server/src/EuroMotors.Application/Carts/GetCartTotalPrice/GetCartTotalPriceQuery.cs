using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.GetCartTotalPrice;

public sealed record GetCartTotalPriceQuery(Guid CartId) : IQuery<decimal>;
