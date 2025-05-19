using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.Extensions.DependencyInjection;
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
        ICommandHandler<DeleteCarModelCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteCarModelCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CarModelErrors.ModelNotFound(command.CarModelId));
    }

    [Fact]
    public async Task Should_DeleteCarModel_WhenCarModelExists()
    {
        // Arrange
        ICommandHandler<CreateCarBrandCommand, Guid> brandHandler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCarBrandCommand, Guid>>();
        var createBrandCommand = new CreateCarBrandCommand("TestBrand", null);
        Result<Guid> brandResult = await brandHandler.Handle(createBrandCommand, CancellationToken.None);
        Guid brandId = brandResult.Value;

        string model = "Test Model";
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        Guid carModelId = await ServiceProvider.CreateCarModelAsync(brandId, model, startYear, bodyType, engineSpec);

        var command = new DeleteCarModelCommand(carModelId);

        // Act
        ICommandHandler<DeleteCarModelCommand> deleteHandler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteCarModelCommand>>();
        Result result = await deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        IQueryHandler<GetCarModelByIdQuery, CarModelResponse> getHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCarModelByIdQuery, CarModelResponse>>();
        var getCarModelQuery = new GetCarModelByIdQuery(carModelId);
        Result<CarModelResponse> getResult = await getHandler.Handle(getCarModelQuery, CancellationToken.None);

        getResult.IsFailure.ShouldBeTrue();
        getResult.Error.ShouldBe(CarModelErrors.ModelNotFound(carModelId));
    }
}
