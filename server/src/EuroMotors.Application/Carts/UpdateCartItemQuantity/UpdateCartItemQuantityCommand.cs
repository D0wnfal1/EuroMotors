using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Carts.UpdateCartItemQuantity;

public sealed class UpdateCartItemQuantityCommand : ICommand
{
    public Guid? UserId { get; }
    public Guid? SessionId { get; }
    public Guid ProductId { get; }
    public int NewQuantity { get; }

    public UpdateCartItemQuantityCommand(Guid? userId, Guid? sessionId, Guid productId, int newQuantity)
    {
        UserId = userId == Guid.Empty ? null : userId;
        SessionId = sessionId;
        ProductId = productId;
        NewQuantity = newQuantity;
    }
}
