using EuroMotors.Application.Categories.SetCategoryAvailability;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Categories;

public class SetCategoryAvailabilityCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly SetCategoryAvailabilityCommandHandler _handler;

    public SetCategoryAvailabilityCommandHandlerTests()
    {
        _handler = new SetCategoryAvailabilityCommandHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new SetCategoryAvailabilityCommand(categoryId, true);

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
    public async Task Handle_ShouldSetAvailability_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Test Category");

        // Update the category's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(category, categoryId);

        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None)
            .Returns(category);

        var command = new SetCategoryAvailabilityCommand(categoryId, false);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify the category availability has been updated
        typeof(Category)
            .GetProperty("IsAvailable")
            ?.GetValue(category)
            .ShouldBe(false);

        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
