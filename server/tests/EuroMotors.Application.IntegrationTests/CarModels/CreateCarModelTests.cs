using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.Extensions.DependencyInjection;
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
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand");
        var command = new CreateCarModelCommand(
            brandId,
            "Test Model",
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        ICommandHandler<CreateCarModelCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCarModelCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenBrandDoesNotExist()
    {
        // Arrange
        var command = new CreateCarModelCommand(
            Guid.NewGuid(),
            "Test Model",
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        ICommandHandler<CreateCarModelCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCarModelCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid()
    {
        // Arrange
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand");
        var command = new CreateCarModelCommand(
            brandId,
            "Test Model",
            0, // Invalid start year
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
            );

        // Act
        ICommandHandler<CreateCarModelCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCarModelCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
