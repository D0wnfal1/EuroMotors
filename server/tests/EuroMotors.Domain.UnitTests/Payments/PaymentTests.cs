using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;
using EuroMotors.Domain.Payments.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Payments;

public class PaymentTests : BaseTest
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private static readonly Guid TransactionId = Guid.NewGuid();
    private const PaymentStatus Status = PaymentStatus.Pending;
    private const decimal Amount = 299.99m;

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Act
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            Status,
            Amount);

        // Assert
        payment.Id.ShouldNotBe(Guid.Empty);
        payment.OrderId.ShouldBe(OrderId);
        payment.TransactionId.ShouldBe(TransactionId);
        payment.Status.ShouldBe(Status);
        payment.Amount.ShouldBe(Amount);
        payment.AmountRefunded.ShouldBe(0m);
        payment.CreatedAtUtc.ShouldNotBe(DateTime.MinValue);
        payment.RefundedAtUtc.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_RaisePaymentCreatedDomainEvent()
    {
        // Act
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            Status,
            Amount);

        // Assert
        PaymentCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<PaymentCreatedDomainEvent>(payment);
        domainEvent.PaymentId.ShouldBe(payment.Id);
    }

    [Fact]
    public void Refund_Should_UpdateAmountRefundedAndRaisePartialRefundEvent_WhenPartialRefund()
    {
        // Arrange
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            PaymentStatus.Success,
            Amount);

        decimal refundAmount = Amount / 2;

        // Act
        Result result = payment.Refund(refundAmount);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        payment.AmountRefunded.ShouldBe(refundAmount);

        PaymentPartiallyRefundedDomainEvent domainEvent = AssertDomainEventWasPublished<PaymentPartiallyRefundedDomainEvent>(payment);
        domainEvent.PaymentId.ShouldBe(payment.Id);
        domainEvent.TransactionId.ShouldBe(TransactionId);
    }

    [Fact]
    public void Refund_Should_UpdateAmountRefundedAndRaiseFullRefundEvent_WhenFullRefund()
    {
        // Arrange
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            PaymentStatus.Success,
            Amount);

        // Act
        Result result = payment.Refund(Amount);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        payment.AmountRefunded.ShouldBe(Amount);

        PaymentRefundedDomainEvent domainEvent = AssertDomainEventWasPublished<PaymentRefundedDomainEvent>(payment);
        domainEvent.PaymentId.ShouldBe(payment.Id);
        domainEvent.TransactionId.ShouldBe(TransactionId);
    }

    [Fact]
    public void Refund_Should_ReturnFailure_WhenAlreadyRefunded()
    {
        // Arrange
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            PaymentStatus.Success,
            Amount);

        payment.Refund(Amount);

        // Act
        Result result = payment.Refund(1m);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PaymentErrors.AlreadyRefunded);
    }

    [Fact]
    public void Refund_Should_ReturnFailure_WhenRefundAmountExceedsPaymentAmount()
    {
        // Arrange
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            PaymentStatus.Success,
            Amount);

        // Act
        Result result = payment.Refund(Amount + 1m);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(PaymentErrors.NotEnoughFunds);
    }

    [Fact]
    public void ChangeStatus_Should_UpdatePaymentStatus()
    {
        // Arrange
        var payment = Payment.Create(
            OrderId,
            TransactionId,
            PaymentStatus.Pending,
            Amount);

        // Act
        payment.ChangeStatus(PaymentStatus.Success);

        // Assert
        payment.Status.ShouldBe(PaymentStatus.Success);
    }
}

