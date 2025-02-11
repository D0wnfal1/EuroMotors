using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;

namespace EuroMotors.Application.Products.SearchProductsByCarModelId;

public sealed record SearchProductsByCarModelIdQuery(Guid CarModelId) : IQuery<IReadOnlyCollection<ProductResponse>>;
