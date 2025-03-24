namespace EuroMotors.Application.Products.GetProductById;

public sealed record ProductImageResponse(Guid ProductImageId, string Path, Guid ProductId);
