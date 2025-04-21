using EuroMotors.Api.Controllers.Products;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Application.Products.SetProductAvailability;
using EuroMotors.Application.Products.UpdateProduct;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Products;

public class ProductControllerTests
{
    private readonly ISender _sender;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new ProductController(_sender)
        {
            // Set up HttpContext for the controller
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk_WhenProductsFound()
    {
        // Arrange
        var products = new List<ProductResponse>
        {
            new ProductResponse
            {
                Id = Guid.NewGuid(),
                Name = "Test Product",
                Price = 100,
                Discount = 10
            }
        };

        var pagination = new Pagination<ProductResponse>
        {
            PageIndex = 1,
            PageSize = 10,
            Count = 1,
            Data = products
        };

        _sender.Send(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        IActionResult result = await _controller.GetProducts(
            null, null, null, null, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(pagination);

        await _sender.Received(1).Send(
            Arg.Is<GetProductsQuery>(query =>
                query.PageNumber == 1 &&
                query.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProducts_ShouldPassAllParameters_ToQuery()
    {
        // Arrange
        var categoryIds = new List<Guid> { Guid.NewGuid() };
        var carModelIds = new List<Guid> { Guid.NewGuid() };
        string sortOrder = "price_asc";
        string searchTerm = "test";
        int pageNumber = 2;
        int pageSize = 20;

        var products = new List<ProductResponse>();
        var pagination = new Pagination<ProductResponse>
        {
            Data = products,
            PageIndex = pageNumber,
            PageSize = pageSize,
            Count = 0
        };

        _sender.Send(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        await _controller.GetProducts(
            categoryIds, carModelIds, sortOrder, searchTerm,
            CancellationToken.None, pageNumber, pageSize);

        // Assert
        await _sender.Received(1).Send(
            Arg.Is<GetProductsQuery>(query =>
                query.CategoryIds == categoryIds &&
                query.CarModelIds == carModelIds &&
                query.SortOrder == sortOrder &&
                query.SearchTerm == searchTerm &&
                query.PageNumber == pageNumber &&
                query.PageSize == pageSize),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductById_ShouldReturnOk_WhenProductFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new ProductResponse
        {
            Id = id,
            Name = "Test Product",
            Price = 100,
            Discount = 10
        };

        _sender.Send(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(product));

        // Act
        IActionResult result = await _controller.GetProductById(id, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(product);

        await _sender.Received(1).Send(
            Arg.Is<GetProductByIdQuery>(query => query.ProductId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _sender.Send(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ProductResponse>(Error.NotFound("Product.NotFound", "Product not found")));

        // Act
        IActionResult result = await _controller.GetProductById(id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedAtAction_WhenCreationSucceeds()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "New Product",
            VendorCode = "VC123",
            CategoryId = Guid.NewGuid(),
            CarModelId = Guid.NewGuid(),
            Price = 200,
            Discount = 20,
            Stock = 50,
            Specifications = new List<SpecificationRequest>
            {
                new SpecificationRequest ( "Color", "Red" )
            }
        };

        var productId = Guid.NewGuid();
        _sender.Send(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(productId));

        // Act
        IActionResult result = await _controller.CreateProduct(request, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(ProductController.GetProductById));
        createdResult.RouteValues?["id"].ShouldBe(productId);

        createdResult.Value.ShouldBe(productId);

        await _sender.Received(1).Send(
            Arg.Is<CreateProductCommand>(cmd =>
                cmd.Name == request.Name &&
                cmd.VendorCode == request.VendorCode &&
                cmd.CategoryId == request.CategoryId &&
                cmd.CarModelId == request.CarModelId &&
                cmd.Price == request.Price &&
                cmd.Discount == request.Discount &&
                cmd.Stock == request.Stock),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "New Product",
            VendorCode = "VC123",
            CategoryId = Guid.NewGuid(),
            CarModelId = Guid.NewGuid(),
            Price = 200,
            Discount = 20,
            Stock = 50,
            Specifications = new List<SpecificationRequest>()
        };

        var error = Error.Failure("Product.InvalidData", "Invalid product data");
        _sender.Send(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.CreateProduct(request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new ProductRequest
        {
            Name = "Updated Product",
            VendorCode = "VC456",
            CategoryId = Guid.NewGuid(),
            CarModelId = Guid.NewGuid(),
            Price = 250,
            Discount = 25,
            Stock = 40,
            Specifications = new List<SpecificationRequest>
            {
                new SpecificationRequest ("Color", "Blue")
            }
        };

        _sender.Send(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateProduct(id, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _sender.Received(1).Send(
            Arg.Is<UpdateProductCommand>(cmd =>
                cmd.ProductId == id &&
                cmd.Name == request.Name &&
                cmd.VendorCode == request.VendorCode &&
                cmd.CategoryId == request.CategoryId &&
                cmd.CarModelId == request.CarModelId &&
                cmd.Price == request.Price &&
                cmd.Discount == request.Discount &&
                cmd.Stock == request.Stock),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new ProductRequest
        {
            Name = "Updated Product",
            VendorCode = "VC456",
            CategoryId = Guid.NewGuid(),
            CarModelId = Guid.NewGuid(),
            Price = 250,
            Discount = 25,
            Stock = 40,
            Specifications = new List<SpecificationRequest>()
        };

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _sender.Send(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateProduct(id, request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task SetProductAvailability_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new SetCategoryAvailabilityRequest
        {
            IsAvailable = true
        };

        _sender.Send(Arg.Any<SetProductAvailabilityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.SetProductAvailability(id, request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _sender.Received(1).Send(
            Arg.Is<SetProductAvailabilityCommand>(cmd =>
                cmd.ProductId == id &&
                cmd.IsAvailable == request.IsAvailable),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetProductAvailability_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new SetCategoryAvailabilityRequest
        {
            IsAvailable = true
        };

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _sender.Send(Arg.Any<SetProductAvailabilityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.SetProductAvailability(id, request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();

        _sender.Send(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteProduct(id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _sender.Received(1).Send(
            Arg.Is<DeleteProductCommand>(cmd => cmd.ProductId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _sender.Send(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteProduct(id, CancellationToken.None);

        // Assert
        NotFoundObjectResult notFoundResult = result.ShouldBeOfType<NotFoundObjectResult>();
        notFoundResult.Value.ShouldBe(error);
    }
}
