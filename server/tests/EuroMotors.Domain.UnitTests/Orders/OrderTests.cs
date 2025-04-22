using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Orders.Events;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.UnitTests.Infrastructure;
using EuroMotors.Domain.UnitTests.Products;
using EuroMotors.Domain.UnitTests.Users;
using EuroMotors.Domain.Users;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Orders;

public class OrderTests : BaseTest
{
    private static readonly Guid UserId = Guid.NewGuid();
    private const string BuyerName = "John Doe";
    private const string BuyerPhoneNumber = "+1234567890";
    private const string BuyerEmail = "john.doe@example.com";
    private const string ShippingAddress = "123 Main St, City, Country";
    private static readonly DeliveryMethod DeliveryMethod = DeliveryMethod.Delivery;
    private static readonly PaymentMethod PaymentMethod = PaymentMethod.Prepaid;

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Act
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        // Assert
        order.Id.ShouldNotBe(Guid.Empty);
        order.UserId.ShouldBe(UserId);
        order.BuyerName.ShouldBe(BuyerName);
        order.BuyerPhoneNumber.ShouldBe(BuyerPhoneNumber);
        order.BuyerEmail.ShouldBe(BuyerEmail);
        order.Status.ShouldBe(OrderStatus.Pending);
        order.DeliveryMethod.ShouldBe(DeliveryMethod);
        order.ShippingAddress.ShouldBe(ShippingAddress);
        order.PaymentMethod.ShouldBe(PaymentMethod);
        order.OrderItems.Count.ShouldBe(0);
        order.TotalPrice.ShouldBe(0);
        order.CreatedAtUtc.ShouldNotBe(DateTime.MinValue);
        order.UpdatedAtUtc.ShouldNotBe(DateTime.MinValue);
    }

    [Fact]
    public void Create_Should_RaiseOrderCreatedDomainEvent()
    {
        // Act
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        // Assert
        OrderCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<OrderCreatedDomainEvent>(order);
        domainEvent.OrderId.ShouldBe(order.Id);
    }

    [Fact]
    public void AddItem_Should_AddOrderItemAndUpdateTotalPrice()
    {
        // Arrange
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        Product product = CreateTestProduct();
        decimal unitPrice = 100.00m;
        int quantity = 2;

        // Act
        order.AddItem(product, quantity, unitPrice);

        // Assert
        order.OrderItems.Count.ShouldBe(1);
        order.TotalPrice.ShouldBe(unitPrice * quantity);

        OrderItem orderItem = order.OrderItems.First();
        orderItem.OrderId.ShouldBe(order.Id);
        orderItem.ProductId.ShouldBe(product.Id);
        orderItem.Quantity.ShouldBe(quantity);
        orderItem.UnitPrice.ShouldBe(unitPrice);
        orderItem.Price.ShouldBe(unitPrice * quantity);
    }

    [Fact]
    public void AddMultipleItems_Should_UpdateTotalPriceCorrectly()
    {
        // Arrange
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        Product product1 = CreateTestProduct();
        Product product2 = CreateTestProduct();

        decimal unitPrice1 = 100.00m;
        int quantity1 = 2;
        decimal unitPrice2 = 50.00m;
        int quantity2 = 3;

        // Act
        order.AddItem(product1, quantity1, unitPrice1);
        order.AddItem(product2, quantity2, unitPrice2);

        // Assert
        order.OrderItems.Count.ShouldBe(2);
        order.TotalPrice.ShouldBe(unitPrice1 * quantity1 + unitPrice2 * quantity2);
    }

    [Fact]
    public void ChangeStatus_Should_UpdateStatusAndUpdatedAtTime()
    {
        // Arrange
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        DateTime originalUpdatedAtUtc = order.UpdatedAtUtc;

        // Act
        order.ChangeStatus(OrderStatus.Completed);

        // Assert
        order.Status.ShouldBe(OrderStatus.Completed);
        order.UpdatedAtUtc.ShouldBeGreaterThan(originalUpdatedAtUtc);
    }

    [Fact]
    public void SetDelivery_Should_UpdateDeliveryInfoAndRaiseEvent()
    {
        // Arrange
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        DeliveryMethod newDeliveryMethod = DeliveryMethod.Pickup;
        string newAddress = "456 New St, New City, Country";

        DateTime originalUpdatedAtUtc = order.UpdatedAtUtc;

        // Act
        order.SetDelivery(newDeliveryMethod, newAddress);

        // Assert
        order.DeliveryMethod.ShouldBe(newDeliveryMethod);
        order.ShippingAddress.ShouldBe(newAddress);
        order.UpdatedAtUtc.ShouldBeGreaterThan(originalUpdatedAtUtc);

        OrderDeliveryMethodChangedDomainEvent domainEvent = AssertDomainEventWasPublished<OrderDeliveryMethodChangedDomainEvent>(order);
        domainEvent.Id.ShouldBe(order.Id);
        domainEvent.DeliveryMethod.ShouldBe(newDeliveryMethod);
    }

    [Fact]
    public void SetPayment_Should_UpdatePaymentIdAndUpdatedAtTime()
    {
        // Arrange
        var order = Order.Create(
            UserId,
            BuyerName,
            BuyerPhoneNumber,
            BuyerEmail,
            DeliveryMethod,
            ShippingAddress,
            PaymentMethod);

        var paymentId = Guid.NewGuid();
        DateTime originalUpdatedAtUtc = order.UpdatedAtUtc;

        // Act
        order.SetPayment(paymentId);

        // Assert
        order.PaymentId.ShouldBe(paymentId);
        order.UpdatedAtUtc.ShouldBeGreaterThan(originalUpdatedAtUtc);
    }

    private static Product CreateTestProduct()
    {
        return Product.Create(
            "Test Product",
            null,
            "TP123",
            Guid.NewGuid(),
            new List<CarModel>(),
            100.00m,
            0m,
            10);
    }

    [Fact]
    public void CreateOrder_ShouldRaiseOrderCreatedDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, "BuyerName", "BuyerPhoneNumber", "BuyerEmail", DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);

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
        var order = Order.Create(user.Id, "BuyerName", "BuyerPhoneNumber", "BuyerEmail", DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var carBrand = CarBrand.Create("Test Brand");
        var carModels = ProductData.CarModelIds
            .Select(id => CarModel.Create(carBrand, "ModelName", 2023, BodyType.Sedan, new EngineSpec(1, FuelType.Diesel)))
            .ToList();
        var product = Product.Create(ProductData.Name, null, ProductData.VendorCode, ProductData.CategoryId, carModels, ProductData.Price,
            ProductData.Discount, ProductData.Stock);

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
        var order = Order.Create(user.Id, "BuyerName", "BuyerPhoneNumber", "BuyerEmail", DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);

        // Act
        order.ChangeStatus(OrderStatus.Shipped);

        // Assert
        Assert.Equal(OrderStatus.Shipped, order.Status);
    }
}
