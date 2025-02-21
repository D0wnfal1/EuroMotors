namespace EuroMotors.Application.Carts;

public class CartItemResponse
{
    public Guid CartItemId { get; }
    public Guid ProductId { get; }
    public Guid CartId { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public CartItemResponse() { }
}

