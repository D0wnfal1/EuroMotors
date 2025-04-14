using Bogus;
using EuroMotors.Application.Categories.SetCategoryAvailability;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class ArchiveCategoryTests : BaseIntegrationTest
{
    public ArchiveCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new SetCategoryAvailabilityCommand(Guid.NewGuid(), false);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CategoryErrors.NotFound(command.CategoryId));
    }

    [Fact]
    public async Task Should_ArchiveCategory_WhenCategoryExists()
    {
        // Arrange
        var faker = new Faker();
        Guid CategoryId = await Sender.CreateCategoryAsync(faker.Music.Genre());

        var command = new SetCategoryAvailabilityCommand(CategoryId, false);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryAlreadyArchived()
    {
        // Arrange
        var faker = new Faker();
        Guid CategoryId = await Sender.CreateCategoryAsync(faker.Music.Genre());

        var command = new SetCategoryAvailabilityCommand(CategoryId, false);

        await Sender.Send(command);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CategoryErrors.AlreadyArchived);
    }
}
