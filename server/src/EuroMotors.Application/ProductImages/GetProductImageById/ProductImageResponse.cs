namespace EuroMotors.Application.ProductImages.GetProductImageById;

public sealed record ProductImageResponse(Guid Id, Uri Url, Guid ProductId);
