using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.ProductImages.CreateProductImage;

public sealed record CreateProductImageCommand(Uri Url, Guid ProductId) : ICommand<Guid>;
