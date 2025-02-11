using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.UpdateProductImage;

public sealed record UpdateProductImageCommand(Guid Id, Uri Url, Guid ProductId) : ICommand;
