using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.ProductImages.UpdateProductImage;

public sealed record UpdateProductImageCommand(Guid Id, IFormFile File, Guid ProductId) : ICommand;
