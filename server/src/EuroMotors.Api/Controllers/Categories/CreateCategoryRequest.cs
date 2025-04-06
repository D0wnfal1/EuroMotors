namespace EuroMotors.Api.Controllers.Categories;

public sealed record CreateCategoryRequest(string Name, Guid? ParentCategoryId, List<string>? SubcategoryNames, IFormFile? Image);
