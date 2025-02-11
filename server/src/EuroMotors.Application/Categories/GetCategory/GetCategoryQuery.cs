using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.GetCategory;

public sealed record GetCategoryQuery(Guid CategoryId) : IQuery<CategoryResponse>;
