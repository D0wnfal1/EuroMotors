using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;

namespace EuroMotors.Application.Products.GetProductsByCategoryId;

public sealed record GetProductsByCategoryIdQuery(Guid CategoryId) : IQuery<IReadOnlyCollection<ProductResponse>>;
