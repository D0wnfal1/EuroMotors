using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem(Guid id, Guid orderId, Guid productId, decimal quantity, decimal unitPrice) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Price = quantity * unitPrice;
    }


    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Price { get; private set; }

    internal static OrderItem Create(Guid orderId, Guid ticketTypeId, decimal quantity, decimal unitPrice)
    {
        return new OrderItem(
            Guid.NewGuid(),
            orderId,
            ticketTypeId,
            quantity,
            unitPrice
        );
    }
}
