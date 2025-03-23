using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels;

public static class CarModelErrors
{
    public static Error NotFound(Guid carModelId) =>
        Error.NotFound("CarModel.NotFound", $"The model of car with the identifier {carModelId} was not found");

    public static Error InvalidPath(string path) =>
        Error.Failure("CarModel.InvalidPath", $"The path '{path}' provided for the product image is invalid.");
}
