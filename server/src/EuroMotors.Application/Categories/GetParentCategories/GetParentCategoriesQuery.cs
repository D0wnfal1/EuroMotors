using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;

namespace EuroMotors.Application.Categories.GetParentCategories;

public sealed record GetParentCategoriesQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<CategoryResponse>>;
