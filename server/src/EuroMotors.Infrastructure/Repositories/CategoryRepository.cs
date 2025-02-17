using EuroMotors.Domain.Categories;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task Delete(Guid categoryId)
    {
        Category? category = await GetByIdAsync(categoryId);

        if (category is null)
        {
            return;
        }

        _dbContext.Remove(category);
        await _dbContext.SaveChangesAsync();
    }
}
