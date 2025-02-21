using EuroMotors.Domain.Orders;

namespace EuroMotors.Domain.Carts;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Cart?> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    void Insert(Cart cart);

    void Update(Cart cart);
    Task AddItemToCartAsync(CartItem cartItem, CancellationToken cancellationToken);

    Task UpdateCartItemQuantityAsync(Guid cartId, Guid productId, int newQuantity, CancellationToken cancellationToken);

    Task RemoveItemFromCartAsync(CartItem cartItem);

    Task ClearCartAsync(Guid cartId, CancellationToken cancellationToken);
}
