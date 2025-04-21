using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.UpdateCarModel;

public sealed record UpdateCarModelCommand(
    Guid Id,
    string ModelName,
    int? StartYear = null,
    BodyType? BodyType = null,
    float? EngineVolumeLiters = null,
    FuelType? EngineFuelType = null) : ICommand;
