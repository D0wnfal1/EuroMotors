using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Products.UpdateProductCarModels;

public sealed record UpdateProductCarModelsCommand(Guid ProductId, List<Guid> CarModelIds) : ICommand;