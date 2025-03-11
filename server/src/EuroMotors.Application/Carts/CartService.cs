using EuroMotors.Application.Abstractions.Caching;

namespace EuroMotors.Application.Carts;

public sealed class CartService(ICacheService cacheService)
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromDays(30);

    public async Task<Cart> GetAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        
        string cacheKey = CreateCacheKey(cartId);

        Cart cart = await cacheService.GetAsync<Cart>(cacheKey, cancellationToken) ??
                    Cart.CreateDefault(cartId); 

        return cart;
    }

    public async Task ClearAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(cartId);

        var cart = Cart.CreateDefault(cartId);

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task AddItemAsync(Guid cartId, CartItem cartItem, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(cartId);

        Cart cart = await GetAsync(cartId, cancellationToken);

        CartItem? existingCartItem = cart.CartItems.Find(c => c.ProductId == cartItem.ProductId);

        if (existingCartItem is null)
        {
            cart.CartItems.Add(cartItem);
        }
        else
        {
            existingCartItem.Quantity += cartItem.Quantity;
        }

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task UpdateQuantityAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(cartId);

        Cart cart = await GetAsync(cartId, cancellationToken);

        CartItem? cartItem = cart.CartItems.Find(c => c.ProductId == productId);

        if (cartItem is null)
        {
            return;
        }

        cartItem.Quantity += quantity;

        if (cartItem.Quantity <= 0)
        {
            cart.CartItems.Remove(cartItem);
        }

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    public async Task RemoveItemAsync(Guid cartId, Guid productId, CancellationToken cancellationToken = default)
    {
        string cacheKey = CreateCacheKey(cartId);

        Cart cart = await GetAsync(cartId, cancellationToken);

        CartItem? cartItem = cart.CartItems.Find(c => c.ProductId == productId);

        if (cartItem is null)
        {
            return;
        }

        cart.CartItems.Remove(cartItem);

        await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
    }

    private static string CreateCacheKey(Guid cartId) => $"carts:{cartId}";
}
