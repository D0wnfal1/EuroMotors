namespace EuroMotors.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Insert(Product product);
}
