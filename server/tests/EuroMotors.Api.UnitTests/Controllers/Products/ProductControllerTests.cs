using EuroMotors.Api.Controllers.Products;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Application.Products.SetProductAvailability;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Domain.Categories;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Products;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly IQueryHandler<GetProductsQuery, Pagination<ProductResponse>> _getProductsHandler;
    private readonly IQueryHandler<GetProductByIdQuery, ProductResponse> _getProductByIdHandler;
    private readonly ICommandHandler<CreateProductCommand, Guid> _createProductHandler;
    private readonly ICommandHandler<UpdateProductCommand> _updateProductHandler;
    private readonly ICommandHandler<SetProductAvailabilityCommand> _setProductAvailabilityHandler;
    private readonly ICommandHandler<DeleteProductCommand> _deleteProductHandler;

    public ProductControllerTests()
    {
        _getProductsHandler = Substitute.For<IQueryHandler<GetProductsQuery, Pagination<ProductResponse>>>();
        _getProductByIdHandler = Substitute.For<IQueryHandler<GetProductByIdQuery, ProductResponse>>();
        _createProductHandler = Substitute.For<ICommandHandler<CreateProductCommand, Guid>>();
        _updateProductHandler = Substitute.For<ICommandHandler<UpdateProductCommand>>();
        _setProductAvailabilityHandler = Substitute.For<ICommandHandler<SetProductAvailabilityCommand>>();
        _deleteProductHandler = Substitute.For<ICommandHandler<DeleteProductCommand>>();

        _controller = new ProductController()
        {
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

        _getProductsHandler.Handle(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        IActionResult result = await _controller.GetProducts(
            _getProductsHandler, null, null, null, null, false, false, false, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(pagination);

        await _getProductsHandler.Received(1).Handle(
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

        _getProductsHandler.Handle(Arg.Any<GetProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        await _controller.GetProducts(
            _getProductsHandler,
            categoryIds, carModelIds, sortOrder, searchTerm, false, false, false,
            CancellationToken.None, pageNumber, pageSize);

        // Assert
        await _getProductsHandler.Received(1).Handle(
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

        _getProductByIdHandler.Handle(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(product));

        // Act
        IActionResult result = await _controller.GetProductById(_getProductByIdHandler, id, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(product);

        await _getProductByIdHandler.Received(1).Handle(
            Arg.Is<GetProductByIdQuery>(query => query.ProductId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNotFound_WhenProductNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _getProductByIdHandler.Handle(Arg.Any<GetProductByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ProductResponse>(Error.NotFound("Product.NotFound", "Product not found")));

        // Act
        IActionResult result = await _controller.GetProductById(_getProductByIdHandler, id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedAtAction_WhenCreationSucceeds()
    {
        // Arrange
        var request = new CreateProductRequest()
        {
            Name = "New Product",
            VendorCode = "VC123",
            CategoryId = Guid.NewGuid(),
            CarModelIds = new List<Guid> { Guid.NewGuid() },
            Price = 200,
            Discount = 20,
            Stock = 50,
            Specifications = new List<SpecificationRequest>
            {
                new SpecificationRequest ( "Color", "Red" )
            }
        };

        var productId = Guid.NewGuid();
        _createProductHandler.Handle(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(productId));

        // Act
        IActionResult result = await _controller.CreateProduct(request, _createProductHandler, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(ProductController.GetProductById));
        createdResult.RouteValues?["id"].ShouldBe(productId);

        createdResult.Value.ShouldBe(productId);

        await _createProductHandler.Received(1).Handle(
            Arg.Is<CreateProductCommand>(cmd =>
                cmd.Name == request.Name &&
                cmd.VendorCode == request.VendorCode &&
                cmd.CategoryId == request.CategoryId &&
                cmd.CarModelIds.SequenceEqual(request.CarModelIds) &&
                cmd.Price == request.Price &&
                cmd.Discount == request.Discount &&
                cmd.Stock == request.Stock),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var request = new CreateProductRequest()
        {
            Name = "New Product",
            VendorCode = "VC123",
            CategoryId = Guid.NewGuid(),
            CarModelIds = new List<Guid> { Guid.NewGuid() },
            Price = 200,
            Discount = 20,
            Stock = 50,
            Specifications = new List<SpecificationRequest>()
        };

        var error = Error.Failure("Product.InvalidData", "Invalid product data");
        _createProductHandler.Handle(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.CreateProduct(request, _createProductHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var category = Category.Create("Category98");
        var request = new UpdateProductRequest
        {
            Name = "Updated Product",
            VendorCode = "VC456",
            CategoryId = category.Id,
            Price = 250,
            Discount = 25,
            Stock = 40,
            Specifications = new List<SpecificationRequest>
            {
                new SpecificationRequest ("Color", "Blue")
            }
        };

        _updateProductHandler.Handle(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateProduct(id, request, _updateProductHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _updateProductHandler.Received(1).Handle(
            Arg.Is<UpdateProductCommand>(cmd =>
                cmd.Name == request.Name &&
                cmd.VendorCode == request.VendorCode &&
                cmd.CategoryId == request.CategoryId &&
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
        var request = new UpdateProductRequest
        {
            Name = "Updated Product",
            VendorCode = "VC456",
            CategoryId = Guid.NewGuid(),
            Price = 250,
            Discount = 25,
            Stock = 40,
            Specifications = new List<SpecificationRequest>()
        };

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _updateProductHandler.Handle(Arg.Any<UpdateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateProduct(id, request, _updateProductHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task SetProductAvailability_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new SetProductAvailabilityRequest()
        {
            IsAvailable = true
        };

        _setProductAvailabilityHandler.Handle(Arg.Any<SetProductAvailabilityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.SetProductAvailability(id, request, _setProductAvailabilityHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _setProductAvailabilityHandler.Received(1).Handle(
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
        var request = new SetProductAvailabilityRequest()
        {
            IsAvailable = true
        };

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _setProductAvailabilityHandler.Handle(Arg.Any<SetProductAvailabilityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.SetProductAvailability(id, request, _setProductAvailabilityHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();

        _deleteProductHandler.Handle(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteProduct(id, _deleteProductHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _deleteProductHandler.Received(1).Handle(
            Arg.Is<DeleteProductCommand>(cmd => cmd.ProductId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var error = Error.NotFound("Product.NotFound", "Product not found");
        _deleteProductHandler.Handle(Arg.Any<DeleteProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteProduct(id, _deleteProductHandler, CancellationToken.None);

        // Assert
        NotFoundObjectResult notFoundResult = result.ShouldBeOfType<NotFoundObjectResult>();
        notFoundResult.Value.ShouldBe(error);
    }
}
