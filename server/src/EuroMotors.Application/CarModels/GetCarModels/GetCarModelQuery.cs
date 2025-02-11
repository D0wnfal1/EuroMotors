using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModel;

namespace EuroMotors.Application.CarModels.GetCarModels;

public sealed record GetCarModelQuery() : IQuery<IReadOnlyCollection<CarModelResponse>>;
