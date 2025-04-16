using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.CarModels.GetCarModelSelection;

public sealed record GetCarModelSelectionQuery(string? Brand, string? Model, int? StartYear, string? BodyType)
    : IQuery<CarModelSelectionResponse>;

