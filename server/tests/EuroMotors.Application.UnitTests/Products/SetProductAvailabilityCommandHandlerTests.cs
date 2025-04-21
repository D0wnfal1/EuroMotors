using EuroMotors.Application.Products.SetProductAvailability;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.Products;

public class SetProductAvailabilityCommandHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly SetProductAvailabilityCommandHandler _handler;

    public SetProductAvailabilityCommandHandlerTests()
    {
        _handler = new SetProductAvailabilityCommandHandler(
            _productRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new SetProductAvailabilityCommand(productId, true);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns((Product)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("Product.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldSetProductAvailabilityToTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new SetProductAvailabilityCommand(productId, true);

        var product = Product.Create(
            "TestProduct",
            new[] { ("Color", "Red"), ("Engine", "V8") },
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            10,
            50);

        product.SetAvailability(false);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.IsAvailable.ShouldBeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldSetProductAvailabilityToFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new SetProductAvailabilityCommand(productId, false);

        var product = Product.Create(
            "TestProduct",
            new[] { ("Color", "Red"), ("Engine", "V8") },
            "VendorCode",
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            10,
            50);

        product.SetAvailability(true);

        _productRepository.GetByIdAsync(productId, CancellationToken.None).Returns(product);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.IsAvailable.ShouldBeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
