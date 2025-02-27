namespace EuroMotors.Application.Carts;

public class CartItem
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice => UnitPrice * Quantity;

}
