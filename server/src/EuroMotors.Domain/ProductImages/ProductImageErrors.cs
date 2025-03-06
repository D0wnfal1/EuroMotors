using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.ProductImages;

public static class ProductImageErrors
{
    public static Error InvalidUrl(Uri url) =>
        Error.Failure("ProductImage.InvalidUrl", $"The URL '{url}' provided for the product image is invalid.");

    public static Error ProductImageNotFound(Guid productImageId) =>
        Error.NotFound("ProductImage.ProductImageNotFound", $"The product image with ID {productImageId} was not found.");

    public static Error ProductImagesNotFound(Guid productId) =>
        Error.NotFound("ProductImages.ProductImagesNotFound", $"No images found for the product with ID {productId}.");
}
