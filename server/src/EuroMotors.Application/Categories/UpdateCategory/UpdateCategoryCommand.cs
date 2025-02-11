using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid CategoryId, string Name) : ICommand;
