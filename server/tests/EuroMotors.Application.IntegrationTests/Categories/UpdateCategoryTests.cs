using Bogus;
using EuroMotors.Application.Categories.UpdateCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class UpdateCategoryTests : BaseIntegrationTest
{
    public UpdateCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    public static readonly TheoryData<UpdateCategoryCommand> InvalidCommands =
    [
        new(Guid.Empty, new Faker().Music.Genre(), null, null),
        new(Guid.NewGuid(), string.Empty, null, null),
        null
    ];

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var faker = new Faker();
        var command = new UpdateCategoryCommand(Guid.NewGuid(), faker.Music.Genre(), null, null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CategoryErrors.NotFound(command.CategoryId));
    }

    [Fact]
    public async Task Should_UpdateCategory_WhenCategoryExists()
    {
        // Arrange
        var faker = new Faker();
        Guid CategoryId = await Sender.CreateCategoryAsync(faker.Music.Genre());

        var command = new UpdateCategoryCommand(CategoryId, faker.Music.Genre(), null, null);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
