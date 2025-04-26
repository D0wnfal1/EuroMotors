using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.GetProductById;

namespace EuroMotors.Application.Products.GetProductsByCategoryWithChildren;

public sealed record GetProductsByCategoryWithChildrenQuery(
    Guid CategoryId,
    string? SortOrder,
    string? SearchTerm,
    int PageNumber,
    int PageSize) : IQuery<Pagination<ProductResponse>>; 
