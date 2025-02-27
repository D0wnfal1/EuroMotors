using EuroMotors.Domain.Abstractions;
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
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems?.ToList();
    public Guid? PaymentId { get; }

    public static Order Create(User user)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Status = OrderStatus.Pending,
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
}
