using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.CarModels;

namespace EuroMotors.Application.CarModels.CreateCarModel;

public sealed record CreateCarModelCommand(
    Guid CarBrandId,
    string ModelName,
    int StartYear,
    BodyType BodyType,
    EngineSpec EngineSpec) : ICommand<Guid>;
