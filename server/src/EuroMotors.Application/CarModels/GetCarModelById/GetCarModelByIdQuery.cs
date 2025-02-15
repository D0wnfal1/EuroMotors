using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.GetCarModelById;

public sealed record GetCarModelByIdQuery(Guid CarModelId) : IQuery<CarModelResponse>;
