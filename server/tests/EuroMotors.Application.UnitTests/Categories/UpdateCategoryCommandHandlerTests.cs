using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Categories.UpdateCategory;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Categories;

public class UpdateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new UpdateCategoryCommandHandler(_categoryRepository, cacheService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand(
            categoryId,
            "Updated Category",
            null,
            null);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns((Category)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Category.NotFound");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Original Category");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(category, categoryId);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns(category);

        var command = new UpdateCategoryCommand(
            categoryId,
            "Updated Category",
            null,
            null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        category.Name.ShouldBe("Updated Category");

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenParentCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentCategoryId = Guid.NewGuid();

        var category = Category.Create("Original Category");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(category, categoryId);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns(category);

        _categoryRepository.GetByIdAsync(parentCategoryId, CancellationToken.None)
            .Returns((Category)null);

        var command = new UpdateCategoryCommand(
            categoryId,
            "Updated Category",
            parentCategoryId,
            null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Category.NotFound");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldSetParentCategory_WhenParentCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentCategoryId = Guid.NewGuid();

        var category = Category.Create("Original Category");
        var parentCategory = Category.Create("Parent Category");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(category, categoryId);

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(parentCategory, parentCategoryId);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns(category);

        _categoryRepository.GetByIdAsync(parentCategoryId, CancellationToken.None)
            .Returns(parentCategory);

        var command = new UpdateCategoryCommand(
            categoryId,
            "Updated Category",
            parentCategoryId,
            null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        category.Name.ShouldBe("Updated Category");
        typeof(Category)
            .GetProperty("ParentCategoryId")
            ?.GetValue(category)
            .ShouldBe(parentCategoryId);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
