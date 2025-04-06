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
        var command = new CreateCarModelCommand("Car Brand", "Car Model", 2020, null, BodyType.Sedan, new EngineSpec(6, FuelType.Diesel, 6), null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid()
    {
        // Arrange
        var command = new CreateCarModelCommand("Car Brand", "Car Model", 0, null, BodyType.Sedan, new EngineSpec(6, FuelType.Diesel, 6), null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
