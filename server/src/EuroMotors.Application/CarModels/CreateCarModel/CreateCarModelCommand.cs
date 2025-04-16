using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.CarModels.CreateCarModel;

public sealed record CreateCarModelCommand(string Brand, string Model, int StartYear, BodyType BodyType, EngineSpec EngineSpec, IFormFile? Image) : ICommand<Guid>;
