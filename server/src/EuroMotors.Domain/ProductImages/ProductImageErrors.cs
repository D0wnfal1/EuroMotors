using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Domain.ProductImages;

public static class ProductImageErrors
{
    public static Error InvalidUrl(string url) =>
        Error.Failure("ProductImage.InvalidUrl", $"The URL '{url}' provided for the product image is invalid.");

    public static Error InvalidFile(IFormFile file) =>
        Error.Failure("ProductImage.InvalidFile", $"The file '{file}' provided for the product image is invalid.");

    public static Error ProductImageNotFound(Guid productImageId) =>
        Error.NotFound("ProductImage.ProductImageNotFound", $"The product image with ID {productImageId} was not found.");

    public static Error ProductImagesNotFound(Guid productId) =>
        Error.NotFound("ProductImages.ProductImagesNotFound", $"No images found for the product with ID {productId}.");
}
