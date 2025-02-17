using EuroMotors.Domain.Products;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task Delete(Guid id)
    {
        Product? product = await GetByIdAsync(id);

        if (product is null)
        {
            return;
        }

        _dbContext.Remove(product);
        await _dbContext.SaveChangesAsync();
    }
}
