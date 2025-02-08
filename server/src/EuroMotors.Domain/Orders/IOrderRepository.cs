namespace EuroMotors.Domain.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Order?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Insert(Order order);
}
