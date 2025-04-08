namespace EuroMotors.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdWithOderItemsAsync(Guid orderId, CancellationToken cancellationToken = default);

    Task<Order?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddItemsToOrder(Order order, CancellationToken cancellationToken = default);

    void Insert(Order order);

    Task Delete(Guid orderId, CancellationToken cancellationToken = default);
}
