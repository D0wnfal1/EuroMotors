using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.ArchiveCategory;

public sealed record ArchiveCategoryCommand(Guid CategoryId) : ICommand;
