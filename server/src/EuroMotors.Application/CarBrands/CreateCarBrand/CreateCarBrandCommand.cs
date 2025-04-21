using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarBrands.CreateCarBrand;

public sealed record CreateCarBrandCommand(string Name, IFormFile? Logo) : ICommand<Guid>;