using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.DeleteImage;

public sealed record DeleteCategoryImageCommand(Guid Id) : ICommand;
