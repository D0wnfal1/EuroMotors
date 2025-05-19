using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
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
        IQueryHandler<GetPaymentByIdQuery, PaymentResponse> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetPaymentByIdQuery, PaymentResponse>>();
        Result<PaymentResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
    }
}
