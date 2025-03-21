using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.ProductImages.UploadProductImage;

public sealed record UploadProductImageCommand(IFormFile File, Guid ProductId) : ICommand<Guid>;

