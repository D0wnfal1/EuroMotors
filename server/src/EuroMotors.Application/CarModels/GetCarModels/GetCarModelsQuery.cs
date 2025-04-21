using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarModels.GetCarModelById;

namespace EuroMotors.Application.CarModels.GetCarModels;

public sealed record GetCarModelsQuery(
    Guid? BrandId = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 10) : IQuery<Pagination<CarModelResponse>>;
