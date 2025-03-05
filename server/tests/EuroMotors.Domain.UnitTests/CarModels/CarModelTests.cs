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
        // Act
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model);

        // Assert
        CarModelCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<CarModelCreatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, domainEvent.CarModelId);
    }

    [Fact]
    public void Update_ShouldChangeBrand_AndRaiseDomainEvent()
    {
        // Arrange
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model);
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
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model);
        var newUrl = new Uri("https://example.com/image.jpg");

        // Act
        Result result = carModel.UpdateImage(newUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newUrl, carModel.ImageUrl);
        CarModelImageUpdatedDomainEvent imageUpdatedEvent = AssertDomainEventWasPublished<CarModelImageUpdatedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, imageUpdatedEvent.CarModelId);
    }

    [Fact]
    public void DeleteImage_ShouldReturn_Success_WhenImageIsDeleted()
    {
        // Arrange
        var carModel = CarModel.Create(CarModelData.Brand, CarModelData.Model);
        carModel.UpdateImage(new Uri("https://example.com/image.jpg"));

        // Act
        Result result = carModel.DeleteImage();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(carModel.ImageUrl);
        CarModelImageDeletedDomainEvent imageDeletedEvent = AssertDomainEventWasPublished<CarModelImageDeletedDomainEvent>(carModel);
        Assert.Equal(carModel.Id, imageDeletedEvent.CarModelId);
    }
}
