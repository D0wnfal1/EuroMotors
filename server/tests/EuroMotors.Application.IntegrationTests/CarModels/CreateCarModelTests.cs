using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class CreateCarModelTests : BaseIntegrationTest
{
    public CreateCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateCarModel_WhenCommandIsValid()
    {
        // Arrange
        await CleanDatabaseAsync();
        Guid brandId = await Sender.CreateCarBrandAsync("TestBrand1");
        var command = new CreateCarModelCommand(
            brandId,
            "X5",
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenBrandDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        var command = new CreateCarModelCommand(
            Guid.NewGuid(),
            "X5",
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid()
    {
        // Arrange
        await CleanDatabaseAsync();
        Guid brandId = await Sender.CreateCarBrandAsync("TestBrand2");
        var command = new CreateCarModelCommand(
            brandId,
            "X5",
            0,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
