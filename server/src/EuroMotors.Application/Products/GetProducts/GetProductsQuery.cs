using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProduct;

namespace EuroMotors.Application.Products.GetProducts;

public sealed record GetProductsQuery() : IQuery<IReadOnlyCollection<ProductResponse>>;
