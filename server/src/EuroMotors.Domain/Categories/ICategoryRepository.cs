namespace EuroMotors.Domain.Category;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Add(Category category);
}
