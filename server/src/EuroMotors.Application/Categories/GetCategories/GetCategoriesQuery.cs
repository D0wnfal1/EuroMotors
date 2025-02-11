using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetCategory;

namespace EuroMotors.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyCollection<CategoryResponse>>;
