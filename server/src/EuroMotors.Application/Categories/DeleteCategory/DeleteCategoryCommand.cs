using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : ICommand;
