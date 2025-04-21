using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarBrands.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;

namespace EuroMotors.Domain.UnitTests.CarModels;

public class CarBrandTests : BaseTest
{
    [Fact]
    public void Create_ShouldReturn_CarBrandCreatedEvent()
    {
        // Arrange
        string brandName = "BMW";

        // Act
        var carBrand = CarBrand.Create(brandName);

        // Assert
        CarBrandCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<CarBrandCreatedDomainEvent>(carBrand);
        Assert.Equal(carBrand.Id, domainEvent.CarBrandId);
        Assert.Equal(brandName, carBrand.Name);
    }

    [Fact]
    public void Update_ShouldChangeName_AndRaiseDomainEvent()
    {
        // Arrange
        var carBrand = CarBrand.Create("BMW");

        string newName = "Audi";

        // Act
        carBrand.Update(newName);

        // Assert
        Assert.Equal(newName, carBrand.Name);
        CarBrandNameChangedDomainEvent nameChangedEvent = AssertDomainEventWasPublished<CarBrandNameChangedDomainEvent>(carBrand);
        Assert.Equal(carBrand.Id, nameChangedEvent.CarBrandId);
        Assert.Equal(newName, nameChangedEvent.Name);
    }

    [Fact]
    public void SetLogoPath_ShouldReturn_Success_WhenValidPath()
    {
        // Arrange
        var carBrand = CarBrand.Create("BMW");

        string logoPath = "/images/brands/bmw.png";

        // Act
        Result result = carBrand.SetLogoPath(logoPath);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(logoPath, carBrand.LogoPath);
        CarBrandLogoUpdatedDomainEvent logoUpdatedEvent = AssertDomainEventWasPublished<CarBrandLogoUpdatedDomainEvent>(carBrand);
        Assert.Equal(carBrand.Id, logoUpdatedEvent.CarBrandId);
    }

    [Fact]
    public void SetLogoPath_ShouldReturn_Failure_WhenInvalidPath()
    {
        // Arrange
        var carBrand = CarBrand.Create("BMW");

        string invalidPath = "";

        // Act
        Result result = carBrand.SetLogoPath(invalidPath);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(carBrand.LogoPath);
        Assert.Empty(carBrand.DomainEvents.OfType<CarBrandLogoUpdatedDomainEvent>());
    }

    [Fact]
    public void Slug_ShouldBeGenerated_FromName()
    {
        // Arrange
        string brandName = "Mercedes Benz";

        // Act
        var carBrand = CarBrand.Create(brandName);

        // Assert
        Assert.Equal("mercedes-benz", carBrand.Slug.Value);
    }
}
