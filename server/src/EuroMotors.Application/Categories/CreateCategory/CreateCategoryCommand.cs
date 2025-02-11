using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : ICommand<Guid>;
