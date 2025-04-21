using Bogus;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class GetCarModelTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public GetCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        var query = new GetCarModelByIdQuery(Guid.NewGuid());

        // Act
        Result<CarModelResponse> result = await Sender.Send(query);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Should_ReturnCarModel_WhenCarModelExists()
    {
        // Arrange
        Guid brandId = await Sender.CreateCarBrandAsync("Test Brand11111");
        Guid carModelId = await Sender.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            EuroMotors.Domain.CarModels.BodyType.Sedan,
            new EuroMotors.Domain.CarModels.EngineSpec(
                _faker.Random.Int(3, 12),
                EuroMotors.Domain.CarModels.FuelType.Diesel)
            );

        var query = new GetCarModelByIdQuery(carModelId);

        // Act
        Result<CarModelResponse> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(carModelId);
    }
}
