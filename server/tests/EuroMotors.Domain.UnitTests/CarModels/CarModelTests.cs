using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.CarModels.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;

namespace EuroMotors.Domain.UnitTests.CarModels;

public class CarModelTests : BaseTest
{
    [Fact]
    public void Create_ShouldReturn_CarModelCreatedEvent()
    {
        // Arrange
        CarBrand carBrand = CarModelData.CreateTestBrand();
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        // Act
        var carModel = CarModel.Create(carBrand, CarModelData.ModelName, startYear, bodyType, engineSpec);

        // Assert
        CarModelCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<CarModelCreatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, domainEvent.CarModelId);
        Assert.Equal(carBrand.Id, carModel.CarBrandId);
        Assert.Equal(CarModelData.ModelName, carModel.ModelName);
    }

    [Fact]
    public void Update_ShouldChangeModelName_AndRaiseDomainEvent()
    {
        // Arrange
        CarBrand carBrand = CarModelData.CreateTestBrand();
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        // Act
        var carModel = CarModel.Create(carBrand, CarModelData.ModelName, startYear, bodyType, engineSpec);

        string newModelName = "Civic";

        // Act
        carModel.Update(newModelName);

        // Assert
        Assert.Equal(newModelName, carModel.ModelName);
        CarModelModelChangedDomainEvent modelChangedEvent = AssertDomainEventWasPublished<CarModelModelChangedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, modelChangedEvent.CarModelId);
        Assert.Equal(newModelName, modelChangedEvent.ModelName);
    }

    [Fact]
    public void Update_ShouldChangeStartYear_AndRaiseDomainEvent()
    {
        // Arrange
        CarBrand carBrand = CarModelData.CreateTestBrand();
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        // Act
        var carModel = CarModel.Create(carBrand, CarModelData.ModelName, startYear, bodyType, engineSpec);

        int newStartYear = 2022;

        // Act
        carModel.Update(CarModelData.ModelName, newStartYear);

        // Assert
        Assert.Equal(newStartYear, carModel.StartYear);
        CarModelStartYearChangedDomainEvent startYearChangedEvent = AssertDomainEventWasPublished<CarModelStartYearChangedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, startYearChangedEvent.CarModelId);
        Assert.Equal(newStartYear, startYearChangedEvent.StartYear);
    }

    [Fact]
    public void Update_ShouldChangeBodyType_AndRaiseDomainEvent()
    {
        // Arrange
        CarBrand carBrand = CarModelData.CreateTestBrand();
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        // Act
        var carModel = CarModel.Create(carBrand, CarModelData.ModelName, startYear, bodyType, engineSpec);

        BodyType newBodyType = BodyType.SUV;

        // Act
        carModel.Update(CarModelData.ModelName, null, newBodyType);

        // Assert
        Assert.Equal(newBodyType, carModel.BodyType);
        CarModelBodyTypeChangedDomainEvent bodyTypeChangedEvent = AssertDomainEventWasPublished<CarModelBodyTypeChangedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, bodyTypeChangedEvent.CarModelId);
        Assert.Equal(newBodyType, bodyTypeChangedEvent.BodyType);
    }

    [Fact]
    public void UpdateEngineSpec_ShouldChangeEngineSpec_AndRaiseDomainEvent()
    {
        // Arrange
        CarBrand carBrand = CarModelData.CreateTestBrand();
        int startYear = 2020;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        // Act
        var carModel = CarModel.Create(carBrand, CarModelData.ModelName, startYear, bodyType, engineSpec);

        float newVolumeLiters = 2.0f;
        FuelType newFuelType = FuelType.Diesel;

        // Act
        carModel.UpdateEngineSpec(newVolumeLiters, newFuelType);

        // Assert
        Assert.Equal(newVolumeLiters, carModel.EngineSpec.VolumeLiters);
        Assert.Equal(newFuelType, carModel.EngineSpec.FuelType);
        CarModelEngineSpecUpdatedDomainEvent engineSpecUpdatedEvent = AssertDomainEventWasPublished<CarModelEngineSpecUpdatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, engineSpecUpdatedEvent.CarModelId);
    }
}
