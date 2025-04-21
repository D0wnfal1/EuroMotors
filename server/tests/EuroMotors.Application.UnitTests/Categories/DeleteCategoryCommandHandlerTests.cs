using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _handler = new DeleteCategoryCommandHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var command = new DeleteCategoryCommand(Guid.NewGuid());

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None)
            .Returns((Category)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Category.NotFound");

        await _categoryRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldDeleteCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new DeleteCategoryCommand(categoryId);

        var category = Category.Create("Test Category");

        // Update the category's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(category, categoryId);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns(category);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _categoryRepository.Received(1).Delete(categoryId);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
