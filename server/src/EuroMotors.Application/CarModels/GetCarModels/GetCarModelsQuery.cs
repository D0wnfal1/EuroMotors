using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarModels.GetCarModelById;

namespace EuroMotors.Application.CarModels.GetCarModels;

public sealed record GetCarModelsQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<CarModelResponse>>;
