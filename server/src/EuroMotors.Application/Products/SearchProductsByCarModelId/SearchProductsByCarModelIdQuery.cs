using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.SearchProductsByCarModelId;

public sealed record SearchProductsByCarModelIdQuery(Guid CarModelId) : IQuery<IReadOnlyCollection<ProductResponse>>;
