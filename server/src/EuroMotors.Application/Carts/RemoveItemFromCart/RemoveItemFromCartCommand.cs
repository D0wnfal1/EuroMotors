using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.RemoveItemFromCart;

public sealed class RemoveItemFromCartCommand : ICommand
{
    public Guid? UserId { get; }
    public Guid? SessionId { get; }
    public Guid ProductId { get; }

    public RemoveItemFromCartCommand(Guid? userId, Guid? sessionId, Guid productId)
    {
        UserId = userId == Guid.Empty ? null : userId;
        SessionId = sessionId;
        ProductId = productId;
    }
}
