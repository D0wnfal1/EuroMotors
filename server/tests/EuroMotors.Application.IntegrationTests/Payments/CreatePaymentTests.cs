using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Payments.CreatePayment;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
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
        ICommandHandler<CreatePaymentCommand, Dictionary<string, string>> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreatePaymentCommand, Dictionary<string, string>>>();
        Result<Dictionary<string, string>> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }
}
