using Bogus;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class UpdateCarModelTests : BaseIntegrationTest
{
    public UpdateCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    public static readonly TheoryData<UpdateCarModelCommand> InvalidCommands =
    [
        new(Guid.Empty, new Faker().Vehicle.Manufacturer(),
            new Faker().Vehicle.Model(), null, null, null, null, null, null),
        new(Guid.NewGuid(), string.Empty, string.Empty, null, null, null, null, null, null)
    ];


    [Theory]
    [MemberData(nameof(InvalidCommands))]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid(UpdateCarModelCommand command)
    {
        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        var faker = new Faker();
        var command = new UpdateCarModelCommand(Guid.NewGuid(), faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(), null, null, null, null, null, null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CarModelErrors.NotFound(command.CarModelId));
    }

    [Fact]
    public async Task Should_UpdateCarModel_WhenCarModelExists()
    {
        // Arrange
        var faker = new Faker();
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            2020,
            null,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        var command = new UpdateCarModelCommand(carModelId, faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(), 2020, null, BodyType.Sedan, null, null, null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
