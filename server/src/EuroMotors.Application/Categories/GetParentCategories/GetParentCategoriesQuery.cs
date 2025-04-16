using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;

namespace EuroMotors.Application.Categories.GetParentCategories;

public sealed record GetParentCategoriesQuery() : IQuery<List<CategoryResponse>>;
