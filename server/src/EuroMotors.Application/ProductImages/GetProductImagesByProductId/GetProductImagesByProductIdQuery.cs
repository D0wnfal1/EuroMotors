using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImages.GetProductImage;

namespace EuroMotors.Application.ProductImages.GetProductImagesByProductId;

public sealed record GetProductImagesByProductIdQuery(Guid ProductId) : IQuery<IReadOnlyCollection<ProductImageResponse>>;
