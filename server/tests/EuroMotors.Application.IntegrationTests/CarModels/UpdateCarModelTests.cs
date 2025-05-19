using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class UpdateCarModelTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UpdateCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        var command = new UpdateCarModelCommand(
            Guid.NewGuid(),
            _faker.Vehicle.Model());

        // Act
        ICommandHandler<UpdateCarModelCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateCarModelCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Should_UpdateCarModel_WhenCarModelExists()
    {
        // Arrange
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand1");
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(2.0f, FuelType.Petrol)
        );

        var command = new UpdateCarModelCommand(
            carModelId,
            _faker.Vehicle.Model(),
            2022,
            BodyType.SUV,
            3.0f,
            FuelType.Diesel);

        // Act
        ICommandHandler<UpdateCarModelCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateCarModelCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
