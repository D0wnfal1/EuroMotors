using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class CreateCategoryTests : BaseIntegrationTest
{
    public CreateCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateCategory_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Category name", null, null, null);

        // Act
        ICommandHandler<CreateCategoryCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCategoryCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsNotValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("", null, null, null);

        // Act
        ICommandHandler<CreateCategoryCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<CreateCategoryCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }
}
