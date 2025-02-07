using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _orderItems = [];


    public Order(Guid id, Guid userId, OrderStatus status, decimal totalPrice, bool productsIssued, DateTime createdAtUtc) : base(id)
    {
        UserId = userId;
        Status = status;
        TotalPrice = totalPrice;
        ProductsIssued = productsIssued;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalPrice { get; private set; }

    public bool ProductsIssued { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.ToList();

    public static Order Create(User user)
    {
        var order = new Order(
            Guid.NewGuid(),
            user.Id,
            OrderStatus.Pending,
            0m,
            false,
            DateTime.UtcNow
        );

        order.RaiseDomainEvents(new OrderCreatedDomainEvent(order.Id));

        return order;
    }

    public void AddItem(Product product, decimal quantity, decimal price, string currency)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        var orderItem = OrderItem.Create(Id, product.Id, quantity, price);

        _orderItems.Add(orderItem);

        TotalPrice = _orderItems.Sum(o => o.Price);
    }

    public Result IssueProducts()
    {
        if (ProductsIssued)
        {
            return Result.Failure(OrderErrors.ProductIsNotAvailable);
        }
        ProductsIssued = true;

        RaiseDomainEvents(new OrderTicketsIssuedDomainEvent(Id));

        return Result.Success();
    }
}
