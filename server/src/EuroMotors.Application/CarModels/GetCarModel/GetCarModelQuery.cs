using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.GetCarModel;

public sealed record GetCarModelQuery(Guid CarModelId) : IQuery<CarModelResponse>;
