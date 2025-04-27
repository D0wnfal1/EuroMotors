using EuroMotors.Domain.Products;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product?> GetWithLockAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Product? product = await _dbContext
            .Products
            .FromSqlInterpolated($"""
                                      SELECT * 
                                      FROM products
                                      WHERE id = {id}
                                      FOR UPDATE NOWAIT
                                  """)
            .AsTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return product;
    }

    public async Task<Product?> GetByIdWithCarModelsAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Products
            .Include(p => p.CarModels)
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task Delete(Guid id)
    {
        Product? product = await GetByIdAsync(id);

        if (product is null)
        {
            return;
        }

        _dbContext.Remove(product);
    }
}
