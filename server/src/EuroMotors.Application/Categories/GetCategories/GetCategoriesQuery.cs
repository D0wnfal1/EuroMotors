using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;

namespace EuroMotors.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery() : IQuery<List<CategoryResponse>>;
