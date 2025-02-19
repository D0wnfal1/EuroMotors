using EuroMotors.Domain.Orders;

namespace EuroMotors.Domain.Carts;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Insert(Cart cart);

    void Update(Cart cart);

    Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int newQuantity, CancellationToken cancellationToken);

    Task AddItemToCartAsync(CartItem cartItem, CancellationToken cancellationToken);

    Task RemoveItemFromCartAsync(CartItem cartItem);

    Task ClearCartAsync(Guid cartId, CancellationToken cancellationToken);
}
