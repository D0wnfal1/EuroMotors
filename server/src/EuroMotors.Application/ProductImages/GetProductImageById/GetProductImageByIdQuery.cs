using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.GetProductImageById;

public sealed record GetProductImageByIdQuery(Guid ProductImageId) : IQuery<ProductImageResponse>;
