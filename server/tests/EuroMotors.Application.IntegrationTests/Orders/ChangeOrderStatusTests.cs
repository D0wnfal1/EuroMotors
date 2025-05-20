using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Orders.ChangeOrderStatus;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Orders;

public class ChangeOrderStatusTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public ChangeOrderStatusTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Refunded)]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Canceled)]
    public async Task Should_ChangeOrderStatus_Successfully(OrderStatus newStatus)
    {
        await CleanDatabaseAsync();

        var order = Order.Create(
            null,
            _faker.Person.FullName,
            _faker.Phone.PhoneNumber(),
            _faker.Internet.Email(),
            DeliveryMethod.Delivery,
            _faker.Address.FullAddress(),
            PaymentMethod.Prepaid);

        DbContext.Orders.Add(order);
        await DbContext.SaveChangesAsync();

        var command = new ChangeOrderStatusCommand(order.Id, newStatus);

        // Act
        ICommandHandler<ChangeOrderStatusCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<ChangeOrderStatusCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        Order? updatedOrder = await DbContext.Orders.FindAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
        updatedOrder.Status.ShouldBe(newStatus);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenOrderDoesNotExist()
    {
        var command = new ChangeOrderStatusCommand(Guid.NewGuid(), OrderStatus.Pending);

        // Act
        ICommandHandler<ChangeOrderStatusCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<ChangeOrderStatusCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(OrderErrors.NotFound(command.OrderId));
    }

    [Fact]
    public void Should_ReturnFailure_WhenStatusChangeIsInvalid()
    {
        var invalidStatusTransitionError = Error.Failure(
            "Order.InvalidStatusTransition",
            "Cannot change order status from Canceled to Pending"
        );

        invalidStatusTransitionError.Code.ShouldBe("Order.InvalidStatusTransition");
    }
}
