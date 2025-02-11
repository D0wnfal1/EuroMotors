using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Domain.Carts;

namespace EuroMotors.Application.Carts;

public sealed class CartService(ICacheService cacheService)
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(20);

    public async Task<Cart> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(userId);

        Cart cart = await cacheService.GetAsync<Cart>(cacheKey, cancellationToken) ??
                    Cart.Create(userId);

        return cart;
    }

    public async Task ClearAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(userId);

        var cart = Cart.Create(userId);

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task AddItemAsync(Guid userId, CartItem cartItem, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(userId);

        Cart cart = await GetAsync(userId, cancellationToken);

        CartItem? existingCartItem = cart.CartItems.Find(c => c.ProductId == cartItem.ProductId);

        if (existingCartItem is null)
        {
            cart.CartItems.Add(cartItem);
        }
        else
        {
            existingCartItem.UpdateQuantity(cartItem.Quantity);
        }

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task RemoveItemAsync(Guid userId, Guid ticketTypeId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(userId);

        Cart cart = await GetAsync(userId, cancellationToken);

        CartItem? cartItem = cart.CartItems.Find(c => c.ProductId == ticketTypeId);

        if (cartItem is null)
        {
            return;
        }

        cart.CartItems.Remove(cartItem);

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    private static string CreateCacheKey(Guid customerId) => $"carts:{customerId}";
}
