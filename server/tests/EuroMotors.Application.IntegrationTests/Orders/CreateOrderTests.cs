using EuroMotors.Application.Carts;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Orders;

public class CreateOrderTests : BaseIntegrationTest
{
    public CreateOrderTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenUserDoesNotExist()
    {
        //Arrange
        var command = new CreateOrderCommand(Guid.NewGuid());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.ShouldBe(UserErrors.NotFound(command.UserId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCartIsEmpty()
    {
        //Arrange
        Guid userId = await Sender.CreateUserAsync();

        var command = new CreateOrderCommand(userId);

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.ShouldBe(CartErrors.Empty);
    }
}
