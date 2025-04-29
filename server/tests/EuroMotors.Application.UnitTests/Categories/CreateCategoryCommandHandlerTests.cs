using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Categories;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new CreateCategoryCommandHandler(
            _categoryRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenParentCategoryIsSubcategory()
    {
        // Arrange
        var parentCategoryId = Guid.NewGuid();
        var grandParentCategoryId = Guid.NewGuid();

        var parentCategory = Category.Create("Parent Category", grandParentCategoryId);

        var command = new CreateCategoryCommand(
            "Test Category",
            parentCategoryId,
            null,
            null);

        _categoryRepository.GetByIdAsync(parentCategoryId, CancellationToken.None).Returns(parentCategory);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("ParentCategoryError");
    }

    [Fact]
    public async Task Handle_ShouldCreateCategoryWithoutParent()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            "Test Category",
            null,
            null,
            null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _categoryRepository.Received(1).Insert(Arg.Any<Category>());
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategoryWithImage()
    {
        // Arrange
        IFormFile? mockFile = Substitute.For<IFormFile>();
        var fileContent = new MemoryStream();
        mockFile.OpenReadStream().Returns(fileContent);
        mockFile.FileName.Returns("test.jpg");
        mockFile.Length.Returns(1024);

        var command = new CreateCategoryCommand(
            "Test Category",
            null,
            null,
            mockFile);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _categoryRepository.Received(1).Insert(Arg.Is<Category>(c =>
            c.Name == "Test Category" &&
            c.ImagePath != null));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
