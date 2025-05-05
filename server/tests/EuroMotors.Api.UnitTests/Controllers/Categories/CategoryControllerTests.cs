using EuroMotors.Api.Controllers.Categories;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.Categories.DeleteCategory;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Categories.GetCategories;
using EuroMotors.Application.Categories.UpdateCategory;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Categories;

public class CategoryControllerTests
{
    private readonly ISender _sender;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new CategoryController(_sender)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetCategories_ShouldReturnOk_WhenCategoriesFound()
    {
        // Arrange
        var categories = new List<CategoryResponse>
        {
            new(Guid.NewGuid(), "Test Category", null, null,"test-category")
        };

        _sender.Send(Arg.Any<GetCategoriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(categories));

        // Act
        IActionResult result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(categories);

        await _sender.Received(1).Send(
            Arg.Any<GetCategoriesQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategories_ShouldReturnNotFound_WhenCategoriesNotFound()
    {
        // Arrange
        _sender.Send(Arg.Any<GetCategoriesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<List<CategoryResponse>>(Error.NotFound("Categories.NotFound", "Categories not found")));

        // Act
        IActionResult result = await _controller.GetCategories(CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnOk_WhenCategoryFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = new CategoryResponse(Guid.NewGuid(), "Test Category", null, null, "test-category");

        _sender.Send(Arg.Any<GetCategoryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(category));

        // Act
        IActionResult result = await _controller.GetCategoryById(id, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(category);

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryByIdQuery>(query => query.CategoryId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCategoryById_ShouldReturnNotFound_WhenCategoryNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var error = Error.NotFound("Category.NotFound", "Category not found");

        _sender.Send(Arg.Any<GetCategoryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<CategoryResponse>(error));

        // Act
        IActionResult result = await _controller.GetCategoryById(id, CancellationToken.None);

        // Assert
        NotFoundObjectResult notFoundResult = result.ShouldBeOfType<NotFoundObjectResult>();
        notFoundResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnCreatedAtAction_WhenCreationSucceeds()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            "New Category",
            null,
            new List<string> { "Subcategory1", "Subcategory2" },
            null);

        var categoryId = Guid.NewGuid();

        _sender.Send(Arg.Any<CreateCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(categoryId));

        // Act
        IActionResult result = await _controller.CreateCategory(request, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(CategoryController.GetCategoryById));
        createdResult.RouteValues?["id"].ShouldBe(categoryId);
        createdResult.Value.ShouldBe(categoryId);

        await _sender.Received(1).Send(
            Arg.Is<CreateCategoryCommand>(cmd =>
                cmd.Name == request.Name &&
                cmd.ParentCategoryId == request.ParentCategoryId &&
                cmd.SubcategoryNames == request.SubcategoryNames),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCategory_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var request = new CreateCategoryRequest(
            "New Category",
            null,
            new List<string> { "Subcategory1", "Subcategory2" },
            null);

        var error = Error.Failure("Category.InvalidData", "Invalid category data");

        _sender.Send(Arg.Any<CreateCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.CreateCategory(request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateCategory_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCategoryRequest(
            "Updated Category",
            null,
            null);

        _sender.Send(Arg.Any<UpdateCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateCategory(id, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _sender.Received(1).Send(
            Arg.Is<UpdateCategoryCommand>(cmd =>
                cmd.CategoryId == id &&
                cmd.Name == request.Name &&
                cmd.ParentCategoryId == request.ParentCategoryId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCategory_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCategoryRequest(
            "Updated Category",
            null,
            null);

        var error = Error.NotFound("Category.NotFound", "Category not found");

        _sender.Send(Arg.Any<UpdateCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateCategory(id, request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }


    [Fact]
    public async Task DeleteCategory_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();

        _sender.Send(Arg.Any<DeleteCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteCategory(id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _sender.Received(1).Send(
            Arg.Is<DeleteCategoryCommand>(cmd => cmd.CategoryId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturnNotFound_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var error = Error.NotFound("Category.NotFound", "Category not found");

        _sender.Send(Arg.Any<DeleteCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteCategory(id, CancellationToken.None);

        // Assert
        NotFoundObjectResult notFoundResult = result.ShouldBeOfType<NotFoundObjectResult>();
        notFoundResult.Value.ShouldBe(error);
    }
}
