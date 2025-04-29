using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Products;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateProductCommandHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly CarBrand _carBrand;

    public CreateProductCommandHandlerTests()
    {
        _carBrand = CarBrand.Create("Test Brand");

        // Set the brand ID for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(_carBrand, _brandId);

        ICacheService? cacheService = Substitute.For<ICacheService>();

        _handler = new CreateProductCommandHandler(
            _productRepository,
            _categoryRepository,
            _carModelRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        var command = new CreateProductCommand(
            "TestProduct",
            specifications,
            "VendorCode",
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            100,
            10,
            50);

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns((Category)null);

        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Category.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarModelNotFound()
    {
        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        var command = new CreateProductCommand(
            "TestProduct",
            specifications,
            "VendorCode",
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            100,
            10,
            50);

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns(Category.Create("Test Name"));
        foreach (Guid carModelId in command.CarModelIds)
        {
            _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None).Returns((CarModel)null);
        }
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldCreateProductSuccessfully()
    {
        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        var command = new CreateProductCommand(
            "TestProduct",
            specifications,
            "VendorCode",
            Guid.NewGuid(),
            new List<Guid> { Guid.NewGuid() },
            100,
            10,
            50);

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns(Category.Create("Test Name"));

        var carModel = CarModel.Create(
            _carBrand,
            "Test Model",
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel));

        foreach (Guid carModelId in command.CarModelIds)
        {
            _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None).Returns(carModel);
        }
        Result<Guid> result = await _handler.Handle(command, default);

        result.IsSuccess.ShouldBeTrue();
        await Task.Run(() => _productRepository.Received(1).Insert(Arg.Any<Product>()));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
