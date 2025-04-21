using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarBrands.UpdateCarBrand;

public sealed record UpdateCarBrandCommand(
    Guid CarBrandId,
    string Name,
    IFormFile? Logo) : ICommand;