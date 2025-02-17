using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages.Events;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.ProductImages;

public class ProductImage : Entity
{
    private ProductImage()
    {
    }

    public Uri Url { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; }

    public static ProductImage Create(Uri url, Guid productId)
    {
        var productImage = new ProductImage
        {
            Id = Guid.NewGuid(),
            Url = url,
            ProductId = productId
        };

        productImage.RaiseDomainEvent(new ProductImageCreatedDomainEvent(productImage.Id));

        return productImage;
    }

    public Result UpdateUrl(Uri newUrl)
    {
        if (string.IsNullOrWhiteSpace(newUrl.ToString()))
        {
            return Result.Failure(ProductImageErrors.InvalidUrl(newUrl));
        }

        Url = newUrl;

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
