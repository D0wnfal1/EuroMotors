using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly UpdateProductCommandHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly CarBrand _carBrand;

    public UpdateProductCommandHandlerTests()
    {
        _carBrand = CarBrand.Create("Test Brand");

        // Set the brand ID for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(_carBrand, _brandId);

        ICacheService? cacheService = Substitute.For<ICacheService>();

        _handler = new UpdateProductCommandHandler(
            _productRepository,
            _categoryRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        var specifications = new List<Specification>
        {
            new Specification("Color", "Blue"),
            new Specification("Engine", "V6")
        };

        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "UpdatedProduct",
            specifications,
            "UpdatedVendorCode",
            Guid.NewGuid(),
            200,
            15,
            75);

        _productRepository.GetByIdAsync(command.ProductId, CancellationToken.None).Returns((Product)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Product.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        // Arrange
        var specifications = new List<Specification>
        {
            new Specification("Color", "Blue"),
            new Specification("Engine", "V6")
        };

        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            "UpdatedProduct",
            specifications,
            "UpdatedVendorCode",
            Guid.NewGuid(),
            200,
            15,
            75);

        var product = Product.Create(
            "OriginalProduct",
            new[] { ("Color", "Red"), ("Engine", "V8") },
            "OriginalVendorCode",
            Guid.NewGuid(),
            new List<CarModel>(),
            100,
            10,
            50);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);
        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns((Category)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Category.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldUpdateProductSuccessfully()
    {
        // Arrange
        var specifications = new List<Specification>
        {
            new Specification("Color", "Blue"),
            new Specification("Engine", "V6")
        };

        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var carModelIds = new List<Guid>();
        var command = new UpdateProductCommand(
            productId,
            "UpdatedProduct",
            specifications,
            "UpdatedVendorCode",
            categoryId,
            200,
            15,
            75);

        var product = Product.Create(
            "OriginalProduct",
            new[] { ("Color", "Red"), ("Engine", "V8") },
            "OriginalVendorCode",
            Guid.NewGuid(),
            new List<CarModel>(),
            100,
            10,
            50);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);
        _categoryRepository.GetByIdAsync(categoryId, CancellationToken.None).Returns(Category.Create("Test Category"));


        foreach (Guid carModelId in carModelIds)
        {
            var carModel = CarModel.Create(
                _carBrand,
                "Test Model",
                2020,
                BodyType.Sedan,
                new EngineSpec(6, FuelType.Diesel));

            _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None).Returns(carModel);
        }
        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
