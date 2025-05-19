using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.Categories.GetCategories;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
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

        var query = new GetCategoriesQuery();

        // Act
        IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>>();
        Result<List<CategoryResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnCategory_WhenCategoryExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        var faker = new Faker();
        await ServiceProvider.CreateCategoryAsync(faker.Music.Genre());
        await ServiceProvider.CreateCategoryAsync(faker.Music.Genre());

        var query = new GetCategoriesQuery();

        // Act
        IQueryHandler<GetCategoriesQuery, List<CategoryResponse>> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCategoriesQuery, List<CategoryResponse>>>();
        Result<List<CategoryResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);
    }
}
