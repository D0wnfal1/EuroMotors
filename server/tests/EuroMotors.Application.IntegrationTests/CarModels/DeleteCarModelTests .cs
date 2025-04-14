using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class DeleteCarModelTests : BaseIntegrationTest
{
    public DeleteCarModelTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        var command = new DeleteCarModelCommand(Guid.NewGuid());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CarModelErrors.NotFound(command.CarModelId));
    }

    [Fact]
    public async Task Should_DeleteCarModel_WhenCarModelExists()
    {
        // Arrange
        string brand = "Test Brand";
        string model = "Test Model";
        int startYear = 2020; // Example start year
        int? endYear = null; // Example end year
        BodyType bodyType = BodyType.Sedan; // Example body type
        var engineSpec = new EngineSpec(6, FuelType.Diesel); // Example engine spec
        IFormFile? image = null; // Example image

        Guid carModelId = await Sender.CreateCarModelAsync(brand, model, startYear, endYear, bodyType, engineSpec, image);

        var command = new DeleteCarModelCommand(carModelId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var getCarModelQuery = new GetCarModelByIdQuery(carModelId);
        Result<CarModelResponse> getResult = await Sender.Send(getCarModelQuery);

        getResult.IsFailure.ShouldBeTrue();
        getResult.Error.ShouldBe(CarModelErrors.NotFound(carModelId));
    }
}
