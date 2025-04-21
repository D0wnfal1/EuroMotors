using EuroMotors.Domain.Orders;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Orders;

public class OrderItemTests : BaseTest
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();
    private const int Quantity = 3;
    private const decimal UnitPrice = 49.99m;

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Act
        var orderItem = OrderItem.Create(
            OrderId,
            ProductId,
            Quantity,
            UnitPrice);

        // Assert
        orderItem.Id.ShouldNotBe(Guid.Empty);
        orderItem.OrderId.ShouldBe(OrderId);
        orderItem.ProductId.ShouldBe(ProductId);
        orderItem.Quantity.ShouldBe(Quantity);
        orderItem.UnitPrice.ShouldBe(UnitPrice);
        orderItem.Price.ShouldBe(Quantity * UnitPrice);
    }

    [Fact]
    public void Create_Should_CalculateTotalPriceCorrectly_WithDifferentQuantities()
    {
        // Arrange
        int[] quantities = [1, 5, 10];

        foreach (int qty in quantities)
        {
            // Act
            var orderItem = OrderItem.Create(OrderId, ProductId, qty, UnitPrice);

            // Assert
            orderItem.Price.ShouldBe(qty * UnitPrice);
        }
    }

    [Fact]
    public void Create_Should_CalculateTotalPriceCorrectly_WithDifferentUnitPrices()
    {
        // Arrange
        decimal[] unitPrices = [0.99m, 10.50m, 99.99m];

        foreach (decimal price in unitPrices)
        {
            // Act
            var orderItem = OrderItem.Create(OrderId, ProductId, Quantity, price);

            // Assert
            orderItem.Price.ShouldBe(Quantity * price);
        }
    }
}