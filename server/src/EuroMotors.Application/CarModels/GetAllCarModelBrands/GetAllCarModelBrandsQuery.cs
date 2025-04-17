using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.GetAllCarModelBrands;


public sealed record GetAllCarModelBrandsQuery() : IQuery<List<string>>;
