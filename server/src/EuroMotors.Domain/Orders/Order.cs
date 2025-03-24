using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _orderItems = [];

    private Order() { }

    public Guid? UserId { get; private set; }
    public string BuyerName { get; private set; } 
    public string BuyerPhoneNumber { get; private set; }
    public string BuyerEmail { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Guid? PaymentId { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; }
    public string? ShippingAddress { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static Order Create(Guid? userId, string buyerName, string buyerPhoneNumber, string buyerEmail, DeliveryMethod deliveryMethod, string? shippingAddress, PaymentMethod paymentMethod)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BuyerName = buyerName,
            BuyerPhoneNumber = buyerPhoneNumber,
            BuyerEmail = buyerEmail,
            Status = OrderStatus.Pending,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = shippingAddress,
            PaymentMethod = paymentMethod,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };


        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));

        return order;
    }

    public void AddItem(Product product, int quantity, decimal unitPrice)
    {
        var orderItem = OrderItem.Create(Id, product.Id, quantity, unitPrice);

        _orderItems.Add(orderItem);

        TotalPrice = _orderItems.Sum(o => o.Price);
    }

    public void ChangeStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetDelivery(DeliveryMethod deliveryMethod, string? shippingAddress = null)
    {
        DeliveryMethod = deliveryMethod;
        ShippingAddress = shippingAddress;
        UpdatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new OrderDeliveryMethodChangedDomainEvent(Id, deliveryMethod, shippingAddress));
    }

    public void SetPayment(Guid paymentId)
    {
        PaymentId = paymentId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
