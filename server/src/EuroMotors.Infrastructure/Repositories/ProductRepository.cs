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
        return await _dbContext
            .Products
            .FromSql(
                $"""
                 SELECT id, category_id, car_model_id, name, description, vendor_code, price, discount, stock, is_available, slug
                 FROM products
                 WHERE id = {id}
                 FOR UPDATE NOWAIT
                 """)
            .SingleOrDefaultAsync(cancellationToken);
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
