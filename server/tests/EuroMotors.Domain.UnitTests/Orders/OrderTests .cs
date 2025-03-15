using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.UnitTests.Infrastructure;
using EuroMotors.Domain.UnitTests.Products;
using EuroMotors.Domain.UnitTests.Users;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void CreateOrder_ShouldRaiseOrderCreatedDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);

        // Act
        OrderCreatedDomainEvent domainEvent = BaseTest.AssertDomainEventWasPublished<OrderCreatedDomainEvent>(order);

        // Assert
        Assert.NotNull(domainEvent);
        Assert.Equal(order.Id, domainEvent.OrderId);
    }

    [Fact]
    public void AddItem_ShouldUpdateTotalPrice()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        order.AddItem(product, 2, product.Price);

        // Assert
        Assert.Equal(200m, order.TotalPrice);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdateStatus()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);

        // Act
        order.ChangeStatus(OrderStatus.Shipped);

        // Assert
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }
}

