using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.UpdateImage;

public sealed record UpdateCategoryImageCommand(Guid Id, Uri Url) : ICommand;
