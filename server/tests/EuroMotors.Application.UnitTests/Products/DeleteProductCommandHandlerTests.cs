using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new DeleteProductCommandHandler(
            _productRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns((Product)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Product.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldDeleteProductSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        var product = Product.Create(
            "TestProduct",
            new[] { ("Color", "Red"), ("Engine", "V8") },
            "VendorCode",
            Guid.NewGuid(),
            new List<CarModel>(),
            100,
            10,
            50);

        typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(product, productId);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _productRepository.Received(1).Delete(productId);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

}
