using Bogus;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class GetCarModelsTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public GetCarModelsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyCollection_WhenNoCategoriesExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetCarModelsQuery(null, null, 1, 10);

        // Act
        Result<Pagination<CarModelResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnCarModel_WhenCarModelExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        Guid brandId1 = await Sender.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        await Sender.CreateCarModelAsync(
            brandId1,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(_faker.Random.Int(3, 12), FuelType.Diesel)
        );

        Guid brandId2 = await Sender.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        await Sender.CreateCarModelAsync(
            brandId2,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.SUV,
            new EngineSpec(_faker.Random.Int(3, 12), FuelType.Diesel)
        );

        var query = new GetCarModelsQuery(null, null, 1, 10);

        // Act
        Result<Pagination<CarModelResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }
}
