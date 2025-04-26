using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.CopyProduct;

public sealed record CopyProductCommand(Guid ProductId) : ICommand<Guid>; 