using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarModels.CreateCarModel;

public sealed record CreateCarModelCommand(string Brand, string Model, IFormFile? Image) : ICommand<Guid>;
