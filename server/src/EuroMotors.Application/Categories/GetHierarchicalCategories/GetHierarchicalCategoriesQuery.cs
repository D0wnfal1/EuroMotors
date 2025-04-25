using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;

namespace EuroMotors.Application.Categories.GetHierarchicalCategories;

public sealed record GetHierarchicalCategoriesQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<HierarchicalCategoryResponse>>;
