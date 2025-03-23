using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name, IFormFile? Image) : ICommand<Guid>;
