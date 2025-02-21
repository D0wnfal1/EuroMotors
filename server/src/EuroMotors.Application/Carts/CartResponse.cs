using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts;

public class CartResponse
{
    public Guid Id { get; }
    public Guid? UserId { get; }  
    public Guid? SessionId { get; } 
    public List<CartItemResponse> CartItems { get; }

    public CartResponse(Guid id, Guid? userId = null, Guid? sessionId = null)
    {
        Id = id;
        UserId = userId;
        SessionId = sessionId;
        CartItems = new List<CartItemResponse>();
    }
}

