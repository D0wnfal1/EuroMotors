namespace EuroMotors.Domain.Category;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(Category category);
}
