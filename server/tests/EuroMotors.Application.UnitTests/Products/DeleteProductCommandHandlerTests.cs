using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Domain.Abstractions;
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
        _handler = new DeleteProductCommandHandler(_productRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ProductExists_ShouldDeleteProductAndReturnSuccess()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = Product.Create(
            "TestProduct",
            "Description",
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            10,
            50,
            true);

        typeof(Product).GetProperty(nameof(Product.Id))?.SetValue(product, productId);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);

        var command = new DeleteProductCommand(productId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _productRepository.Received(1).Delete(productId);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
    }


    [Fact]
    public async Task Handle_ProductDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns((Product)null);

        var command = new DeleteProductCommand(productId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _productRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(CancellationToken.None);
        result.IsFailure.ShouldBeTrue();
    }
}
