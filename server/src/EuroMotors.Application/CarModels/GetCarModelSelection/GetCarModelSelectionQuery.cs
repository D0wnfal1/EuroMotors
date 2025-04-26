using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.GetCarModelSelection;

public sealed record GetCarModelSelectionQuery(
    Guid? BrandId = null,
    string? Brand = null,
    string? ModelName = null,
    int? StartYear = null,
    string? BodyType = null)
    : IQuery<CarModelSelectionResponse>;

