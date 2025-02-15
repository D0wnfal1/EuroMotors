using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.GetProductById;

public sealed record GetProductByIdQuery(Guid ProductId) : IQuery<ProductResponse>;
