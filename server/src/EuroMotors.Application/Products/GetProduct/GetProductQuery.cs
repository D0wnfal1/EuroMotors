using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.GetProduct;

public sealed record GetProductQuery(Guid ProductId) : IQuery<ProductResponse>;
