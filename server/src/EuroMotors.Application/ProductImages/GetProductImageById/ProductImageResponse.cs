namespace EuroMotors.Application.ProductImages.GetProductImageById;

public sealed record ProductImageResponse(Guid Id, string Url, Guid ProductId);
