using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;

namespace EuroMotors.Application.Products.GetProductsByCarModelId;

public sealed record GetProductsByCarModelIdQuery(Guid CarModelId) : IQuery<IReadOnlyCollection<ProductResponse>>;
