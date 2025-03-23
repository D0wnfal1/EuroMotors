using Bogus;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Categories.GetCategories;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class GetCategoriesTests : BaseIntegrationTest
{
    public GetCategoriesTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyCollection_WhenNoCategoriesExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetCategoriesQuery(1, 10);

        // Act
        Result<Pagination<CategoryResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnCategory_WhenCategoryExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        var faker = new Faker();
        await Sender.CreateCategoryAsync(faker.Music.Genre());
        await Sender.CreateCategoryAsync(faker.Music.Genre());

        var query = new GetCategoriesQuery(1, 10);

        // Act
        Result<Pagination<CategoryResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }
}
