using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Categories.SetCategoryAvailability;

public sealed record SetCategoryAvailabilityCommand(Guid CategoryId, bool IsAvailable) : ICommand;
