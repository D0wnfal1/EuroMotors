using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class UpdateCarModelCommandHandlerTests
{
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCarModelCommandHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly CarBrand _carBrand;

    public UpdateCarModelCommandHandlerTests()
    {
        _carBrand = CarBrand.Create("BMW");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(_carBrand, _brandId);

        ICacheService? cacheService = Substitute.For<ICacheService>();

        _handler = new UpdateCarModelCommandHandler(_carModelRepository, cacheService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarModelNotFound()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated X5",
            2021,
            BodyType.SUV,
            3.0f,
            FuelType.Diesel);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns((CarModel)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateCarModel_WhenCarModelExists()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        CarModel carModel = CreateTestCarModel(carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated X5",
            2021,
            BodyType.Sedan,
            3.0f,
            FuelType.Diesel);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        carModel.ModelName.ShouldBe("Updated X5");
        carModel.StartYear.ShouldBe(2021);
        carModel.BodyType.ShouldBe(BodyType.Sedan);
        carModel.EngineSpec.VolumeLiters.ShouldBe(3.0f);
        carModel.EngineSpec.FuelType.ShouldBe(FuelType.Diesel);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldUpdateOnlyProvidedFields_WhenPartialUpdateProvided()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        CarModel carModel = CreateTestCarModel(carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated X5");

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        carModel.ModelName.ShouldBe("Updated X5");
        carModel.StartYear.ShouldBe(2020);
        carModel.BodyType.ShouldBe(BodyType.SUV);
        carModel.EngineSpec.VolumeLiters.ShouldBe(2.0f);
        carModel.EngineSpec.FuelType.ShouldBe(FuelType.Petrol);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEngineSpec_WhenEngineSpecProvided()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        CarModel carModel = CreateTestCarModel(carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        var command = new UpdateCarModelCommand(
            carModelId,
            "X5",
            null,
            null,
            3.5f,
            FuelType.Diesel);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        carModel.ModelName.ShouldBe("X5");
        carModel.StartYear.ShouldBe(2020);
        carModel.BodyType.ShouldBe(BodyType.SUV);
        carModel.EngineSpec.VolumeLiters.ShouldBe(3.5f);
        carModel.EngineSpec.FuelType.ShouldBe(FuelType.Diesel);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    private CarModel CreateTestCarModel(Guid id)
    {
        var carModel = CarModel.Create(
            _carBrand,
            "X5",
            2020,
            BodyType.SUV,
            new EngineSpec(2.0f, FuelType.Petrol));

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carModel, id);

        return carModel;
    }
}
