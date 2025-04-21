using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels;

public static class CarModelErrors
{
    public static Error ModelNotFound(Guid carModelId) =>
        Error.NotFound("CarModel.NotFound", $"The model of car with the identifier {carModelId} was not found");

    public static Error BrandNotFound(Guid carBrandId) =>
        Error.NotFound("CarBrand.NotFound", $"The car brand with the identifier {carBrandId} was not found");

    public static Error BrandNameNotFound(string brandName) =>
        Error.NotFound("CarBrand.NameNotFound", $"The car brand with the name '{brandName}' was not found");

    public static Error InvalidPath(string path) =>
        Error.Failure("File.InvalidPath", $"The path '{path}' provided is invalid.");
}
