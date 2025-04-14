using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.SetProductAvailability;

public sealed record SetProductAvailabilityCommand(Guid ProductId, bool IsAvailable) : ICommand;
