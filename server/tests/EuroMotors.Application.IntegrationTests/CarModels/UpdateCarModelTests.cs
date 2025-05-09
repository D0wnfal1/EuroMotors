using Bogus;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class UpdateCarModelTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UpdateCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    public static readonly TheoryData<UpdateCarModelCommand> InvalidCommands =
    [
        new(Guid.Empty, string.Empty),
        new(Guid.NewGuid(), string.Empty)
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
        var command = new UpdateCarModelCommand(
            Guid.NewGuid(),
            _faker.Vehicle.Model());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Should_UpdateCarModel_WhenCarModelExists()
    {
        // Arrange
        Guid brandId = await Sender.CreateCarBrandAsync("Test Brand1");
        Guid carModelId = await Sender.CreateCarModelAsync(
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
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

}
