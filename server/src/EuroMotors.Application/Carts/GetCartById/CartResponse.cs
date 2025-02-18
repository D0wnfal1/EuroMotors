using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts.GetCartById;

public class CartResponse
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public List<CartItemResponse> CartItems { get; }

    public CartResponse(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        CartItems = new List<CartItemResponse>();
    }
}

