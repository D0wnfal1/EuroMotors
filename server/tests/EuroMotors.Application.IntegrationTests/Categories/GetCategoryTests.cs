using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Categories.GetByIdCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class GetCategoryTests : BaseIntegrationTest
{
    public GetCategoryTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var query = new GetCategoryByIdQuery(Guid.NewGuid());

        // Act
        IQueryHandler<GetCategoryByIdQuery, CategoryResponse> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCategoryByIdQuery, CategoryResponse>>();
        Result<CategoryResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(CategoryErrors.NotFound(query.CategoryId));
    }

    [Fact]
    public async Task Should_ReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var faker = new Faker();
        Guid CategoryId = await ServiceProvider.CreateCategoryAsync(faker.Music.Genre());

        var query = new GetCategoryByIdQuery(CategoryId);

        // Act
        IQueryHandler<GetCategoryByIdQuery, CategoryResponse> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCategoryByIdQuery, CategoryResponse>>();
        Result<CategoryResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
