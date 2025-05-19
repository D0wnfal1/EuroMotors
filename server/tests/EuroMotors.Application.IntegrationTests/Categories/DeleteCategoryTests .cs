using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Microsoft.Extensions.DependencyInjection;
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
        ICommandHandler<DeleteCategoryCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteCategoryCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NotFound(command.CategoryId));
    }

    [Fact]
    public async Task Should_DeleteCategory_WhenCategoryExists()
    {
        // Arrange
        string CategoryName = "Test Category";
        Guid CategoryId = await ServiceProvider.CreateCategoryAsync(CategoryName);

        var command = new DeleteCategoryCommand(CategoryId);

        // Act
        ICommandHandler<DeleteCategoryCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteCategoryCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var getCategoryQuery = new GetCategoryByIdQuery(CategoryId);
        IQueryHandler<GetCategoryByIdQuery, CategoryResponse> getHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCategoryByIdQuery, CategoryResponse>>();
        Result<CategoryResponse> getResult = await getHandler.Handle(getCategoryQuery, CancellationToken.None);

        getResult.IsFailure.ShouldBeTrue();
        getResult.Error.ShouldBe(CategoryErrors.NotFound(CategoryId));
    }
}
