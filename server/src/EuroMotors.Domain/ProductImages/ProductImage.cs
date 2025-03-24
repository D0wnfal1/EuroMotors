using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.ProductImages;

public class ProductImage : Entity
{
    private ProductImage()
    {
    }

    public string Path { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; }

    public static ProductImage Create(string path, Guid productId)
    {
        var productImage = new ProductImage
        {
            Id = Guid.NewGuid(),
            Path = path,
            ProductId = productId
        };

        productImage.RaiseDomainEvent(new ProductImageCreatedDomainEvent(productImage.Id));

        return productImage;
    }

    public Result UpdateImage(string newImagePath, Guid productId)
    {
        if (string.IsNullOrWhiteSpace(newImagePath))
        {
            return Result.Failure(ProductImageErrors.InvalidPath(newImagePath));
        }

        Path  = newImagePath;
        ProductId = productId;

        RaiseDomainEvent(new ProductImageUpdatedDomainEvent(Id));

        return Result.Success();
    }


    public Result Delete()
    {
        if (ProductId == Guid.Empty)
        {
            return Result.Failure(ProductImageErrors.ProductImageNotFound(Id));
        }

        RaiseDomainEvent(new ProductImageDeletedDomainEvent(Id));

        return Result.Success();
    }
}
