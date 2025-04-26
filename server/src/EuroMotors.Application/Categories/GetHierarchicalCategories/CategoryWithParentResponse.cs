namespace EuroMotors.Application.Categories.GetHierarchicalCategories;

public sealed class CategoryWithParentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string Slug { get; set; } = string.Empty;
}
