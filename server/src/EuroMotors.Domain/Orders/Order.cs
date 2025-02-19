using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _orderItems = [];

    private Order() { }

    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public Guid? PaymentId { get; private set; }

    public static Order Create(Guid userId, List<CartItem> cartItems)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        foreach (CartItem cartItem in cartItems)
        {
            order.AddItem(cartItem.ProductId, cartItem.Quantity, cartItem.UnitPrice);
        }

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));
        return order;
    }

    public void AddItem(Guid productId, decimal quantity, decimal unitPrice)
    {
        var orderItem = OrderItem.Create(Id, productId, quantity, unitPrice);

        _orderItems.Add(orderItem);

        RecalculateTotalPrice();
    }

    public void RecalculateTotalPrice()
    {
        TotalPrice = _orderItems.Sum(o => o.Price);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPaymentId(Guid paymentId)
    {
        PaymentId = paymentId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(OrderStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
