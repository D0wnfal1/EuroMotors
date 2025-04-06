using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    List<string>? SubcategoryNames,
    IFormFile? Image
) : ICommand<Guid>;
