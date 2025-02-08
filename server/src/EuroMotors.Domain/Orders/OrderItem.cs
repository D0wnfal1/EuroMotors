using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem()
    {

    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Price { get; private set; }

    public Product Product { get; private set; } = null!;

    internal static OrderItem Create(Guid orderId, Guid productId, decimal quantity, decimal unitPrice)
    {
        return new OrderItem()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Price = quantity * unitPrice
        };
    }

    public void UpdateQuantity(decimal quantity)
    {
        Quantity = quantity;
        Price = Quantity * UnitPrice;
    }
}
