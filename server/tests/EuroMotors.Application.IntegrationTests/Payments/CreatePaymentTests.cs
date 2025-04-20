using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Payments.CreatePayment;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Payments;

public class CreatePaymentTests : BaseIntegrationTest
{
    public CreatePaymentTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenOrderDoesNotExist()
    {
        // Arrange
        var nonExistingOrderId = Guid.NewGuid();
        var command = new CreatePaymentCommand(nonExistingOrderId);

        // Act
        Result<Dictionary<string, string>> result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }
}
