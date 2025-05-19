using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class GetCarModelTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();
    private readonly IQueryHandler<GetCarModelByIdQuery, CarModelResponse> _getCarModelByIdHandler;

    public GetCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _getCarModelByIdHandler = factory.Services.GetRequiredService<IQueryHandler<GetCarModelByIdQuery, CarModelResponse>>();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        var query = new GetCarModelByIdQuery(Guid.NewGuid());

        // Act
        Result<CarModelResponse> result = await _getCarModelByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Should_ReturnCarModel_WhenCarModelExists()
    {
        // Arrange
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand11111");
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
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
        Result<CarModelResponse> result = await _getCarModelByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(carModelId);
    }
}
