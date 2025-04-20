using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Payments;

public class GetPaymentByIdTests : BaseIntegrationTest
{
    public GetPaymentByIdTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenPaymentDoesNotExist()
    {
        // Arrange
        var nonExistingPaymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery(nonExistingPaymentId);

        // Act
        Result<PaymentResponse> result = await Sender.Send(query);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }


} 
