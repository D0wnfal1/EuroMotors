namespace EuroMotors.Application.Categories.GetHierarchicalCategories;

public sealed class HierarchicalCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string? ImagePath { get; set; }
    public string Slug { get; set; } = string.Empty;
    public List<HierarchicalCategoryResponse> SubCategories { get; set; } = new();
}
