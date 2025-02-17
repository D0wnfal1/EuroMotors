using EuroMotors.Domain.ProductImages;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public void Update(ProductImage image)
    {
        _dbContext.ProductImages.Update(image);
        _dbContext.SaveChanges();
    }

    public async Task Delete(Guid productImageId)
    {
        ProductImage? productImage = await GetByIdAsync(productImageId);

        if (productImage == null)
        {
            return;
        }

        _dbContext.ProductImages.Remove(productImage);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        List<ProductImage> productImages = await _dbContext.ProductImages
            .Where(pi => pi.ProductId == productId)
            .ToListAsync(cancellationToken);

        _dbContext.ProductImages.RemoveRange(productImages);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
