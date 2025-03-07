using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.SearchProducts;

public record SearchProductsQuery(
    List<Guid>? CategoryIds,
    List<Guid>? CarModelIds,
    string? SortOrder,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<Pagination<ProductResponse>>;


