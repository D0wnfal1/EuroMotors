using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
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
        result.Error.ShouldBe(CarModelErrors.ModelNotFound(command.CarModelId));
    }

    [Fact]
    public async Task Should_DeleteCarModel_WhenCarModelExists()
    {
        // Arrange
        var createBrandCommand = new CreateCarBrandCommand("TestBrand", null);
        Result<Guid> brandResult = await Sender.Send(createBrandCommand);
        Guid brandId = brandResult.Value;

        string model = "Test Model";
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        Guid carModelId = await Sender.CreateCarModelAsync(brandId, model, startYear, bodyType, engineSpec);

        var command = new DeleteCarModelCommand(carModelId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var getCarModelQuery = new GetCarModelByIdQuery(carModelId);
        Result<CarModelResponse> getResult = await Sender.Send(getCarModelQuery);

        getResult.IsFailure.ShouldBeTrue();
        getResult.Error.ShouldBe(CarModelErrors.ModelNotFound(carModelId));
    }

}
