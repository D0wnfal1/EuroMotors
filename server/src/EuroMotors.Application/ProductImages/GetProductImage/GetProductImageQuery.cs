using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.GetProductImage;

public sealed record GetProductImageQuery(Guid ProductImageId) : IQuery<ProductImageResponse>;
