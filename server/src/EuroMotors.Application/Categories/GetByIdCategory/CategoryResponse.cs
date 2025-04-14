namespace EuroMotors.Application.Categories.GetByIdCategory;

public sealed record CategoryResponse(Guid Id, string Name, bool IsAvailable, string? ImagePath, Guid? ParentCategoryId, string Slug);
