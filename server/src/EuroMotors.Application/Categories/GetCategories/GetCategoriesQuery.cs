using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Products.GetProducts;

namespace EuroMotors.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<CategoryResponse>>;
