using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class UpdateCarModelCommandHandlerTests
{
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCarModelCommandHandler _handler;

    public UpdateCarModelCommandHandlerTests()
    {
        _handler = new UpdateCarModelCommandHandler(_carModelRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarModelNotFound()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated BMW",
            "Updated X5",
            2021,
            BodyType.SUV,
            3.0f,
            FuelType.Diesel,
            null);

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
        
        var carModel = CarModel.Create(
            "BMW", 
            "X5", 
            2020, 
            BodyType.SUV, 
            new EngineSpec(2.0f, FuelType.Petrol));
            
        // Update the carModel's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carModel, carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated BMW",
            "Updated X5",
            2021,
            BodyType.Sedan,
            3.0f,
            FuelType.Diesel,
            null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Verify the model has been updated with new values
        carModel.Brand.ShouldBe("Updated BMW");
        carModel.Model.ShouldBe("Updated X5");
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
        
        var carModel = CarModel.Create(
            "BMW", 
            "X5", 
            2020, 
            BodyType.SUV, 
            new EngineSpec(2.0f, FuelType.Petrol));
            
        // Update the carModel's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carModel, carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        // Only updating brand, leaving other fields null or default
        var command = new UpdateCarModelCommand(
            carModelId,
            "Updated BMW",
            "X5",
            null,
            null,
            null,
            null,
            null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Verify only brand was updated
        carModel.Brand.ShouldBe("Updated BMW");
        carModel.Model.ShouldBe("X5");  // unchanged
        carModel.StartYear.ShouldBe(2020);  // unchanged
        carModel.BodyType.ShouldBe(BodyType.SUV);  // unchanged
        carModel.EngineSpec.VolumeLiters.ShouldBe(2.0f);  // unchanged
        carModel.EngineSpec.FuelType.ShouldBe(FuelType.Petrol);  // unchanged
        
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
} 
