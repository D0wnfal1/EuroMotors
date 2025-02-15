using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.SearchProductsByCategoryId;

public sealed record SearchProductsByCategoryIdQuery(Guid CategoryId) : IQuery<IReadOnlyCollection<ProductResponse>>;
