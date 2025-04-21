using EuroMotors.Domain.CarBrands;

namespace EuroMotors.Domain.UnitTests.CarModels;

internal static class CarModelData
{
    public const string BrandName = "Test Brand";
    public const string ModelName = "Test Model";

    public static CarBrand CreateTestBrand() => CarBrand.Create(BrandName);
}
