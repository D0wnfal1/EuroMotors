namespace EuroMotors.Api.Controllers.Categories;

public sealed record UpdateCategoryRequest(string Name, IFormFile? Image);
