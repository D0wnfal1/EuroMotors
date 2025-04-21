using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;

namespace EuroMotors.Application.CarBrands.GetCarBrands;

public sealed record GetCarBrandsQuery(int PageNumber = 1, int PageSize = 10) : IQuery<Pagination<CarBrandResponse>>;