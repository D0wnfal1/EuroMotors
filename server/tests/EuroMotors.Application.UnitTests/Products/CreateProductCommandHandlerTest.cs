using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Domain.Abstractions;
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

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            _productRepository,
            _categoryRepository,
            _carModelRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCategoryNotFound()
    {
        var command = new CreateProductCommand(
            "TestProduct",
            "Description",
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
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
        var command = new CreateProductCommand(
            "TestProduct",
            "Description",
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            10,
            50);

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns(Category.Create("Test Name"));
        _carModelRepository.GetByIdAsync(command.CarModelId, CancellationToken.None).Returns((CarModel)null);

        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldCreateProductSuccessfully()
    {
        var command = new CreateProductCommand(
            "TestProduct",
            "Description",
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            10,
            50);

        _categoryRepository.GetByIdAsync(command.CategoryId, CancellationToken.None).Returns(Category.Create("Test Name"));
        _carModelRepository.GetByIdAsync(command.CarModelId, CancellationToken.None).Returns(CarModel.Create("Test Brand", "Test CarModel", 2020, null, BodyType.Sedan, new EngineSpec(6, FuelType.Diesel)));

        Result<Guid> result = await _handler.Handle(command, default);

        result.IsSuccess.ShouldBeTrue();
        await Task.Run(() => _productRepository.Received(1).Insert(Arg.Any<Product>()));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
