using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class DeleteCategoryTests : BaseIntegrationTest
{
    public DeleteCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NotFound(command.CategoryId));
    }

    [Fact]
    public async Task Should_DeleteCategory_WhenCategoryExists()
    {
        // Arrange
        string CategoryName = "Test Category";
        Guid CategoryId = await Sender.CreateCategoryAsync(CategoryName);

        var command = new DeleteCategoryCommand(CategoryId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var getCategoryQuery = new GetCategoryByIdQuery(CategoryId);
        Result<CategoryResponse> getResult = await Sender.Send(getCategoryQuery);

        getResult.IsFailure.ShouldBeTrue();
        getResult.Error.ShouldBe(CategoryErrors.NotFound(CategoryId));
    }
}
