using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.GetByIdCategory;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryResponse>;
