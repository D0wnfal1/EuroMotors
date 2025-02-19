using System.Threading;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Products;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Cart>()
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
    }

    public async void Update(Cart cart)
    {
        _dbContext.Set<Cart>().Update(cart);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateCartItemQuantityAsync(Guid userId, Guid productId, int newQuantity, CancellationToken cancellationToken)
    {
        Cart? cart = await GetByUserIdAsync(userId, cancellationToken);

        CartItem? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == productId);

        if (cartItem == null)
        {
            return;
        }

        if (newQuantity <= 0)
        {
            return;
        }

        cartItem.UpdateQuantity(newQuantity);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddItemToCartAsync(CartItem cartItem, CancellationToken cancellationToken)
    {
        await _dbContext.CartItems.AddAsync(cartItem, cancellationToken);
    }

    public async Task RemoveItemFromCartAsync(CartItem cartItem)
    {
        Cart? cart = await _dbContext.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == cartItem.CartId);

        CartItem? itemToRemove = cart?.CartItems.FirstOrDefault(x => x.ProductId == cartItem.ProductId);

        if (itemToRemove == null)
        {
            return;
        }

        _dbContext.CartItems.Remove(cartItem);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearCartAsync(Guid cartId, CancellationToken cancellationToken)
    {
        Cart? cart = await _dbContext.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == cartId, cancellationToken);

        if (cart == null)
        {
            return;
        }

        _dbContext.CartItems.RemoveRange(cart.CartItems);

        _dbContext.Remove(cart);
    }
}
