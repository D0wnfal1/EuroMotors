using EuroMotors.Domain.Abstractions;
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
        int startYear = 2020;
        int? endYear = null;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel, 300);

        // Act
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model, startYear, endYear, bodyType, engineSpec);

        // Assert
        CarModelCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<CarModelCreatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, domainEvent.CarModelId);
    }

    [Fact]
    public void Update_ShouldChangeBrand_AndRaiseDomainEvent()
    {
        // Arrange
        int startYear = 2020;
        int? endYear = null;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel, 300);

        // Act
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model, startYear, endYear, bodyType, engineSpec);

        string newBrand = "Honda";
        string newModel = "Civic";

        // Act
        carModel.Update(newBrand, newModel);

        // Assert
        Assert.Equal(newBrand, carModel.Brand);
        Assert.Equal(newModel, carModel.Model);
        CarModelBrandChangedDomainEvent brandChangedEvent = AssertDomainEventWasPublished<CarModelBrandChangedDomainEvent>(carModel);
        CarModelModelChangedDomainEvent modelChangedEvent = AssertDomainEventWasPublished<CarModelModelChangedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, brandChangedEvent.CarModelId);
        Assert.Equal(carModel.Id, modelChangedEvent.CarModelId);
    }

    [Fact]
    public void UpdateImage_ShouldReturn_Success_WhenValidUrl()
    {
        // Arrange
        int startYear = 2020;
        int? endYear = null;
        BodyType bodyType = BodyType.Sedan;
        var engineSpec = new EngineSpec(6, FuelType.Diesel, 300);

        // Act
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model, startYear, endYear, bodyType, engineSpec);

        string newUrl = new("https://example.com/image.jpg");

        // Act
        Result result = carModel.SetImagePath(newUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newUrl, carModel.ImagePath);
        CarModelImageUpdatedDomainEvent imageUpdatedEvent = AssertDomainEventWasPublished<CarModelImageUpdatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, imageUpdatedEvent.CarModelId);
    }

}
