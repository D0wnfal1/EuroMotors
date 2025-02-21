using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.ConvertToOrder;

public sealed class ConvertToOrderCommand : ICommand
{
    public Guid? UserId { get; }
    public Guid? SessionId { get; }

    public ConvertToOrderCommand(Guid? userId, Guid? sessionId)
    {
        UserId = userId == Guid.Empty ? null : userId;
        SessionId = sessionId;
    }
}
