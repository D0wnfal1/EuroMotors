namespace EuroMotors.Api.Controllers.Categories;

public sealed record UpdateCategoryRequest(string Name, Guid? ParentCategoryId, IFormFile? Image);
