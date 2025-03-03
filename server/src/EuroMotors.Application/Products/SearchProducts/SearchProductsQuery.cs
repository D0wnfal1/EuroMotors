using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.SearchProducts;

public record SearchProductsQuery(
    string? CategoryName,
    string? CarModelBrand,
    string? CarModelModel,
    string? SortOrder,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 10
) : IQuery<IReadOnlyCollection<ProductResponse>>;


