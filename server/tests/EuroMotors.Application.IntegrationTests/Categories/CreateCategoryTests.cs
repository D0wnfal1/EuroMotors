using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class CreateCategoryTests : BaseIntegrationTest
{
    public CreateCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateCategory_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Category name", null, null, null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("", null, null, null);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
