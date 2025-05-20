using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Orders;

public class GetOrdersTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public GetOrdersTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenNoOrdersExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetOrdersQuery(
            PageNumber: 1,
            PageSize: 10,
            Status: null);

        // Act
        IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>>();
        Result<Pagination<OrdersResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(0);
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_ReturnOrders_WhenOrdersExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        for (int i = 0; i < 5; i++)
        {
            var order = Order.Create(
                null,
                _faker.Person.FullName,
                _faker.Phone.PhoneNumber(),
                _faker.Internet.Email(),
                DeliveryMethod.Delivery,
                _faker.Address.FullAddress(),
                PaymentMethod.Prepaid);

            DbContext.Orders.Add(order);
        }

        await DbContext.SaveChangesAsync();

        var query = new GetOrdersQuery(
            PageNumber: 1,
            PageSize: 10,
            Status: null);

        // Act
        IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>>();
        Result<Pagination<OrdersResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(5);
        result.Value.Count.ShouldBe(5);
    }

    [Fact]
    public async Task Should_ReturnFilteredOrders_WhenStatusIsSpecified()
    {
        // Arrange
        await CleanDatabaseAsync();

        for (int i = 0; i < 5; i++)
        {
            var order = Order.Create(
                null,
                _faker.Person.FullName,
                _faker.Phone.PhoneNumber(),
                _faker.Internet.Email(),
                DeliveryMethod.Delivery,
                _faker.Address.FullAddress(),
                PaymentMethod.Prepaid);

            DbContext.Orders.Add(order);
        }

        await DbContext.SaveChangesAsync();

        var query = new GetOrdersQuery(
            PageNumber: 1,
            PageSize: 10,
            Status: OrderStatus.Pending);

        // Act
        IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>>();
        Result<Pagination<OrdersResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(5);
        result.Value.Count.ShouldBe(5);

        foreach (OrdersResponse order in result.Value.Data)
        {
            order.Status.ShouldBe(OrderStatus.Pending);
        }
    }

    [Fact]
    public async Task Should_ReturnPaginatedOrders_WhenMoreOrdersExistThanPageSize()
    {
        // Arrange
        await CleanDatabaseAsync();

        for (int i = 0; i < 15; i++)
        {
            var order = Order.Create(
                null,
                _faker.Person.FullName,
                _faker.Phone.PhoneNumber(),
                _faker.Internet.Email(),
                DeliveryMethod.Delivery,
                _faker.Address.FullAddress(),
                PaymentMethod.Postpaid);

            DbContext.Orders.Add(order);
        }

        await DbContext.SaveChangesAsync();

        var query = new GetOrdersQuery(
            PageNumber: 2,
            PageSize: 10,
            Status: null);

        // Act
        IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>>();
        Result<Pagination<OrdersResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(5);
        result.Value.Count.ShouldBe(15);
        result.Value.PageIndex.ShouldBe(2);
        result.Value.PageSize.ShouldBe(10);
    }
}
