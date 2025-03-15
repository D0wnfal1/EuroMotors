using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using EuroMotors.Domain.Payments.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;
using EuroMotors.Domain.UnitTests.Users;
using EuroMotors.Domain.Users;

namespace EuroMotors.Domain.UnitTests.Payments;

public class PaymentTests
{
    [Fact]
    public void CreatePayment_ShouldRaisePaymentCreatedDomainEvent()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Pending, 100m);

        // Act
        PaymentCreatedDomainEvent domainEvent = BaseTest.AssertDomainEventWasPublished<PaymentCreatedDomainEvent>(payment);

        // Assert
        Assert.NotNull(domainEvent);
        Assert.Equal(payment.Id, domainEvent.PaymentId);
    }

    [Fact]
    public void Refund_ShouldReturnSuccess_WhenValidRefundAmount()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Success, 100m);

        // Act
        Result result = payment.Refund(50m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(50m, payment.AmountRefunded);
    }

    [Fact]
    public void Refund_ShouldReturnFailure_WhenRefundAmountExceedsTotalAmount()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Success, 100m);

        // Act
        Result result = payment.Refund(150m);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.NotEnoughFunds, result.Error);
    }

    [Fact]
    public void ChangeStatus_ShouldUpdatePaymentStatus()
    {
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Pending, 100m);

        payment.ChangeStatus(PaymentStatus.Success);

        Assert.Equal(PaymentStatus.Success, payment.Status);
    }

    [Fact]
    public void Refund_ShouldReturnFailure_WhenAlreadyFullyRefunded()
    {
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Success, 100m);
        payment.Refund(100m);

        Result result = payment.Refund(50m);

        Assert.False(result.IsSuccess);
        Assert.Equal(PaymentErrors.AlreadyRefunded, result.Error);
    }

    [Fact]
    public void Refund_ShouldRaisePaymentRefundedDomainEvent_WhenFullyRefunded()
    {
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        var order = Order.Create(user.Id, DeliveryMethod.Pickup, "", PaymentMethod.Postpaid);
        var payment = Payment.Create(order.Id, Guid.NewGuid(), PaymentStatus.Success, 100m);

        payment.Refund(100m);

        PaymentRefundedDomainEvent domainEvent = BaseTest.AssertDomainEventWasPublished<PaymentRefundedDomainEvent>(payment);

        Assert.NotNull(domainEvent);
        Assert.Equal(payment.Id, domainEvent.PaymentId);
    }
}

