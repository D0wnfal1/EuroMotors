namespace EuroMotors.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> GetWithLockAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdWithCarModelsAsync(Guid id, CancellationToken ct);

    void Insert(Product product);

    Task Delete(Guid id);

}
