namespace EuroMotors.Domain.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(Category category);

    Task Delete(Guid categoryId);

    Task Update(Category category);

    IQueryable<Category> GetAll();
}
