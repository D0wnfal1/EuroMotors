using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;

namespace EuroMotors.Application.CarBrands.GetCarBrandById;

public sealed record GetCarBrandByIdQuery(Guid CarBrandId) : IQuery<CarBrandResponse>;