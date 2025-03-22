using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.Products.GetProducts;

namespace EuroMotors.Application.CarModels.GetCarModels;

public sealed record GetCarModelsQuery(int PageNumber,
    int PageSize) : IQuery<Pagination<CarModelResponse>>;
