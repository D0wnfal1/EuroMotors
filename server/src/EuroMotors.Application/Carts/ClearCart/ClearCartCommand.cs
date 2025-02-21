using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.ClearCart;

public sealed class ClearCartCommand : ICommand
{
    public Guid? UserId { get; }
    public Guid? SessionId { get; }

    public ClearCartCommand(Guid? userId, Guid? sessionId)
    {
        UserId = userId == Guid.Empty ? null : userId;
        SessionId = sessionId;
    }
}
