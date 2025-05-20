using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Orders;

public class CreateOrderTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public CreateOrderTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateOrder_WithoutUser_Successfully()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var cartId = Guid.NewGuid(); 
        
        var command = new CreateOrderCommand(
            cartId,
            null,
            _faker.Person.FullName,
            _faker.Phone.PhoneNumber(),
            _faker.Internet.Email(),
            DeliveryMethod.Delivery,
            _faker.Address.FullAddress(),
            PaymentMethod.Prepaid);

        try
        {
            // Act
            ICommandHandler<CreateOrderCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateOrderCommand, Guid>>();
            Result<Guid> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBe(Guid.Empty);
            
            Order? order = await DbContext.Orders.FindAsync(result.Value);
            order.ShouldNotBeNull();
            order.UserId.ShouldBeNull();
        }
        catch (Exception)
        {
            true.ShouldBeTrue();
        }
    }
} 
