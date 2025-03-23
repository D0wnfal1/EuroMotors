using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.GetProducts;

public sealed record GetProductsQuery(
    List<Guid>? CategoryIds,
    List<Guid>? CarModelIds,
    string? SortOrder,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<Pagination<ProductResponse>>;
