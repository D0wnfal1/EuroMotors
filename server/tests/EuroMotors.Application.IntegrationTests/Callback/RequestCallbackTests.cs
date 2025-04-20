using Bogus;
using EuroMotors.Application.Callback.RequestCallback;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Callback;

public class RequestCallbackTests : BaseIntegrationTest
{
    public RequestCallbackTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenRequestingCallback()
    {
        // Arrange
        var faker = new Faker();
        var command = new RequestCallbackCommand(
            faker.Name.FullName(),
            faker.Phone.PhoneNumber("+###########"));

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
} 