using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.MarkAsNotAvailable;

public sealed record MarkAsNotAvailableCommand(Guid ProductId) : ICommand;
