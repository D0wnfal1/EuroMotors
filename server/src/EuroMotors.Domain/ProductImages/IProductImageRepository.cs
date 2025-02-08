namespace EuroMotors.Domain.ProductImages;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Insert(ProductImage image);

    void Update(ProductImage image);

    void Delete(Guid id);
}
