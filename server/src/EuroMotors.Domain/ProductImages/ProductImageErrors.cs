using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.ProductImages;

public static class ProductImageErrors
{
    public static Error InvalidUrl() =>
        Error.Failure("ProductImage.InvalidUrl", "The URL provided for the product image is invalid.");

    public static Error ProductNotFound(Guid productId) =>
        Error.NotFound("ProductImage.ProductNotFound", $"The product with ID {productId} was not found.");

    public static Error UrlUpdateFailed() =>
        Error.Failure("ProductImage.UrlUpdateFailed", "Failed to update the URL for the product image.");

    public static Error ProductImageNotFound(Guid productImageId) =>
        Error.NotFound("ProductImage.NotFound", $"The product image with ID {productImageId} was not found.");

    public static Error ProductImagesNotFound(Guid productId) =>
        Error.NotFound("ProductImages.NotFound", $"No images found for the product with ID {productId}.");
}
