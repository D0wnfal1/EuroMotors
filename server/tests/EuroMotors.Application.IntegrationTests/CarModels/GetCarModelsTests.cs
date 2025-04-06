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
    public GetCarModelsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyCollection_WhenNoCategoriesExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetCarModelsQuery(1, 10);

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

        var faker = new Faker();
        await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year, // startYear
            null, // endYear
            BodyType.Sedan, // Assuming BodyType is an enum
            new EngineSpec(6, FuelType.Diesel, 6), // Assuming EngineSpec is a class
            null // Assuming IFormFile is not needed for this test
        );

        await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year, // startYear
            null, // endYear
            BodyType.SUV, // Assuming BodyType is an enum
            new EngineSpec(6, FuelType.Diesel, 6),  // Assuming EngineSpec is a class
            null // Assuming IFormFile is not needed for this test
        );


        var query = new GetCarModelsQuery(1, 10);

        // Act
        Result<Pagination<CarModelResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }
}
