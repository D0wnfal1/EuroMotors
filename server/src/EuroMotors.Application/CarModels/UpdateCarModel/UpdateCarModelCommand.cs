using EuroMotors.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

public record UpdateCarModelCommand(Guid CarModelId, string Brand, string Model, IFormFile? Image) : ICommand;
