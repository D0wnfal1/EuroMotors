using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _orderItems = [];

    private Order()
    {
    }

    public Guid UserId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    public bool ProductsIssued { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => [];

    public Guid? PaymentId { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public static Order Create(User user)
    {
        var order = new Order()
        {
           Id = Guid.NewGuid(),
           UserId = user.Id,
           Status = OrderStatus.Pending,
           TotalPrice = 0m,
           ProductsIssued = false,
           CreatedAtUtc = DateTime.UtcNow,
           UpdatedAtUtc = DateTime.UtcNow
        };

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));

        return order;
    }

    public void AddItem(Product product, decimal quantity, decimal price)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        var orderItem = OrderItem.Create(Id, product.Id, quantity, price);

        _orderItems.Add(orderItem);

        TotalPrice = _orderItems.Sum(o => o.Price);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecalculateTotalPrice()
    {
        TotalPrice = _orderItems.Sum(o => o.Price);

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public Result IssueProducts()
    {
        if (ProductsIssued)
        {
            return Result.Failure(OrderErrors.ProductIsNotAvailable);
        }
        ProductsIssued = true;

        RaiseDomainEvent(new OrderProductsIssuedDomainEvent(Id));

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public void SetPaymentId(Guid paymentId)
    {
        PaymentId = paymentId;

        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeStatus(OrderStatus status)
    {
        Status = status;
    }
}
