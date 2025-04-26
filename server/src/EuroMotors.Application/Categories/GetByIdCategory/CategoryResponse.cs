namespace EuroMotors.Application.Categories.GetByIdCategory;

public sealed record CategoryResponse(Guid Id, string Name, string? ImagePath, Guid? ParentCategoryId, string Slug);
