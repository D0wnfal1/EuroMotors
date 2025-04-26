using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.GetProducts;

public sealed record GetProductsQuery(
    List<Guid>? CategoryIds,
    List<Guid>? CarModelIds,
    string? SortOrder,
    string? SearchTerm,
    bool? IsDiscounted = null,
    bool? IsNew = null,
    bool? IsPopular = null,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<Pagination<ProductResponse>>;
