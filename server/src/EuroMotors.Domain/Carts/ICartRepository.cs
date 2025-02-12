using EuroMotors.Domain.Orders;

namespace EuroMotors.Domain.Carts;

public interface ICartRepository
{
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Insert(Cart cart);
    void Update(Cart cart, CancellationToken cancellationToken = default);
}
