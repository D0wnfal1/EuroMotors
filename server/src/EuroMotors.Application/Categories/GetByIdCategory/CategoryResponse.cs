namespace EuroMotors.Application.Categories.GetByIdCategory;

public sealed record CategoryResponse(Guid Id, string Name, bool IsArchived, string? ImagePath, Guid? ParentCategoryId, string Slug);
