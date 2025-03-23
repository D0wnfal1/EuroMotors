using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Categories.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;

namespace EuroMotors.Domain.UnitTests.Categories;

public class CategoryTests : BaseTest
{
    [Fact]
    public void Create_ShouldReturn_CategoryCreatedEvent()
    {
        // Act
        var category = Category.Create(CategoryData.Name);

        // Assert
        CategoryCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<CategoryCreatedDomainEvent>(category);
        Assert.Equal(category.Id, domainEvent.CategoryId);
    }

    [Fact]
    public void Archive_ShouldSetIsArchived_AndRaiseDomainEvent()
    {
        // Arrange
        var category = Category.Create(CategoryData.Name);

        // Act
        category.Archive();

        // Assert
        Assert.True(category.IsArchived);
        CategoryArchivedDomainEvent archivedEvent = AssertDomainEventWasPublished<CategoryArchivedDomainEvent>(category);
        Assert.Equal(category.Id, archivedEvent.CategoryId);
    }

    [Fact]
    public void ChangeName_ShouldChangeName_AndRaiseDomainEvent()
    {
        // Arrange
        var category = Category.Create(CategoryData.Name);
        string newName = "Home Appliances";

        // Act
        category.ChangeName(newName);

        // Assert
        Assert.Equal(newName, category.Name);
        CategoryNameChangedDomainEvent nameChangedEvent = AssertDomainEventWasPublished<CategoryNameChangedDomainEvent>(category);
        Assert.Equal(category.Id, nameChangedEvent.CategoryId);
        Assert.Equal(newName, nameChangedEvent.Name);
    }

    [Fact]
    public void UpdateImage_ShouldReturn_Success_WhenValidUrl()
    {
        // Arrange
        var category = Category.Create(CategoryData.Name);
        string newUrl = "https://example.com/Category-image.jpg";

        // Act
        Result result = category.SetImagePath(newUrl);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newUrl, category.ImagePath);
        CategoryImageUpdatedDomainEvent imageUpdatedEvent = AssertDomainEventWasPublished<CategoryImageUpdatedDomainEvent>(category);
        Assert.Equal(category.Id, imageUpdatedEvent.CategoryId);
    }

}
