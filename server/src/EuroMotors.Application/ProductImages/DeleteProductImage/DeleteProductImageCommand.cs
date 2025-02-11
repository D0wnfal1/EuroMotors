using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.DeleteProductImage;

public sealed record DeleteProductImageCommand(Guid Id) : ICommand;
