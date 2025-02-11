using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.DeleteProductImagesByProductId;

public sealed record DeleteProductImageByProductIdCommand(Guid ProductId) : ICommand;
