using EuroMotors.Api.Controllers.ProductImages;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.ProductImages.DeleteProductImage;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Application.ProductImages.UploadProductImage;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.ProductImages;

public class ProductImageControllerTests
{
    private readonly ProductImageController _controller;
    private readonly ICommandHandler<UploadProductImageCommand, Guid> _uploadProductImageHandler;
    private readonly ICommandHandler<UpdateProductImageCommand> _updateProductImageHandler;
    private readonly ICommandHandler<DeleteProductImageCommand> _deleteProductImageHandler;

    public ProductImageControllerTests()
    {
        _uploadProductImageHandler = Substitute.For<ICommandHandler<UploadProductImageCommand, Guid>>();
        _updateProductImageHandler = Substitute.For<ICommandHandler<UpdateProductImageCommand>>();
        _deleteProductImageHandler = Substitute.For<ICommandHandler<DeleteProductImageCommand>>();

        _controller = new ProductImageController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task UploadProductImage_ShouldReturnCreatedAtAction_WhenUploadSucceeds()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var formFile = new FormFile(new MemoryStream(), 0, 0, "Data", "test.jpg");
        var request = new UploadProductImageRequest
        {
            File = formFile,
            ProductId = productId
        };

        var imageId = Guid.NewGuid();
        _uploadProductImageHandler.Handle(Arg.Any<UploadProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(imageId));

        // Act
        IActionResult result = await _controller.UploadProductImage(request, _uploadProductImageHandler, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(ProductImageController.UploadProductImage));
        createdResult.RouteValues?["id"].ShouldBe(imageId);

        await _uploadProductImageHandler.Received(1).Handle(
            Arg.Is<UploadProductImageCommand>(cmd =>
                cmd.File == request.File &&
                cmd.ProductId == request.ProductId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UploadProductImage_ShouldReturnBadRequest_WhenUploadFails()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var formFile = new FormFile(new MemoryStream(), 0, 0, "Data", "test.jpg");
        var request = new UploadProductImageRequest
        {
            File = formFile,
            ProductId = productId
        };

        var error = Error.Failure("ProductImage.UploadFailed", "Failed to upload product image");
        _uploadProductImageHandler.Handle(Arg.Any<UploadProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.UploadProductImage(request, _uploadProductImageHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateProductImage_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var formFile = new FormFile(new MemoryStream(), 0, 0, "Data", "test.jpg");
        var request = new UploadProductImageRequest
        {
            File = formFile,
            ProductId = productId
        };

        _updateProductImageHandler.Handle(Arg.Any<UpdateProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateProductImage(imageId, request, _updateProductImageHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _updateProductImageHandler.Received(1).Handle(
            Arg.Is<UpdateProductImageCommand>(cmd =>
                cmd.Id == imageId &&
                cmd.File == request.File &&
                cmd.ProductId == request.ProductId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductImage_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var imageId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var formFile = new FormFile(new MemoryStream(), 0, 0, "Data", "test.jpg");
        var request = new UploadProductImageRequest
        {
            File = formFile,
            ProductId = productId
        };

        var error = Error.NotFound("ProductImage.NotFound", "Product image not found");
        _updateProductImageHandler.Handle(Arg.Any<UpdateProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateProductImage(imageId, request, _updateProductImageHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteProductImage_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var imageId = Guid.NewGuid();

        _deleteProductImageHandler.Handle(Arg.Any<DeleteProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteProductImage(imageId, _deleteProductImageHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _deleteProductImageHandler.Received(1).Handle(
            Arg.Is<DeleteProductImageCommand>(cmd => cmd.Id == imageId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProductImage_ShouldReturnBadRequest_WhenDeletionFails()
    {
        // Arrange
        var imageId = Guid.NewGuid();

        var error = Error.NotFound("ProductImage.NotFound", "Product image not found");
        _deleteProductImageHandler.Handle(Arg.Any<DeleteProductImageCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteProductImage(imageId, _deleteProductImageHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
