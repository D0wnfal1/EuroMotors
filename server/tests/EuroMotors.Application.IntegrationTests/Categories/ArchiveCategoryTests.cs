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

}
