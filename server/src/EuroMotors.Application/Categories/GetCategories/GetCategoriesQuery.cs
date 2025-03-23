using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;

namespace EuroMotors.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<CategoryResponse>>;
