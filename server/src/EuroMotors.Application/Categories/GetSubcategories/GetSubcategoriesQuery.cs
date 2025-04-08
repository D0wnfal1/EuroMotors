using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;

namespace EuroMotors.Application.Categories.GetSubcategories;

public sealed record GetSubcategoriesQuery(Guid ParentCategoryId) : IQuery<List<CategoryResponse>>;
