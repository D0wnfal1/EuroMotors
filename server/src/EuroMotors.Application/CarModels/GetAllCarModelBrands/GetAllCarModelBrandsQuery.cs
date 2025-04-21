using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;

namespace EuroMotors.Application.CarModels.GetAllCarModelBrands;

public sealed record GetAllCarModelBrandsQuery() : IQuery<List<CarBrandResponse>>;
