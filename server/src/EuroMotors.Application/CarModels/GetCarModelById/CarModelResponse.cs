namespace EuroMotors.Application.CarModels.GetCarModelById;

public sealed record CarModelResponse(Guid Id, string Brand, string Model, string? ImagePath);
