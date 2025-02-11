namespace EuroMotors.Application.ProductImages.GetProductImage;

public sealed record ProductImageResponse(Guid Id, Uri Url, Guid ProductId);
