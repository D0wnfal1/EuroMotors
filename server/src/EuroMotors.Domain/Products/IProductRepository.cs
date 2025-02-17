namespace EuroMotors.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(Product product);

    Task Delete(Guid id);
}
