namespace EuroMotors.Domain.ProductImages;

public interface IProductImageRepository
{
    Task<ProductImage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

    void Insert(ProductImage image);

    void Update(ProductImage image);

    void Delete(Guid productImageId);

    Task DeleteByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
