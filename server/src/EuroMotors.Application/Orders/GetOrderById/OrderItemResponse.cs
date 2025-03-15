namespace EuroMotors.Application.Orders.GetOrderById;

public sealed class OrderItemResponse
{
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Price { get; set; }

    public OrderItemResponse() { }
}
