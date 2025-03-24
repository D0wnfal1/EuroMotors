namespace EuroMotors.Application.ProductImages.GetProductImageById;

public sealed record ProductImageResponse(Guid Id, string Path, Guid ProductId);
