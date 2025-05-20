using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarBrands;

public class GetCarBrandsTests : BaseIntegrationTest
{
    public GetCarBrandsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenNoBrandsExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetCarBrandsQuery(1, 10);

        // Act
        IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>>>();
        Result<Pagination<CarBrandResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(0);
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_ReturnPaginatedResults_WhenMultipleBrandsExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        string[] brandNames = new[] { "Audi", "BMW", "Chevrolet", "Dodge", "Ferrari" };
        foreach (string name in brandNames)
        {
            var brand = CarBrand.Create(name);
            await DbContext.CarBrands.AddAsync(brand);
        }
        await DbContext.SaveChangesAsync();

        var query1 = new GetCarBrandsQuery(1, 2);

        // Act - First page
        IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>>>();
        Result<Pagination<CarBrandResponse>> result1 = await handler.Handle(query1, CancellationToken.None);

        // Assert - First page
        result1.IsSuccess.ShouldBeTrue();
        result1.Value.Data.Count.ShouldBe(2);
        result1.Value.Count.ShouldBe(5);
        result1.Value.PageIndex.ShouldBe(1);
        result1.Value.PageSize.ShouldBe(2);

        var query2 = new GetCarBrandsQuery(2, 2);
        Result<Pagination<CarBrandResponse>> result2 = await handler.Handle(query2, CancellationToken.None);

        result2.IsSuccess.ShouldBeTrue();
        result2.Value.Data.Count.ShouldBe(2);
        result2.Value.Count.ShouldBe(5);
        result2.Value.PageIndex.ShouldBe(2);

        IEnumerable<Guid> page1Ids = result1.Value.Data.Select(b => b.Id);
        IEnumerable<Guid> page2Ids = result2.Value.Data.Select(b => b.Id);
        page1Ids.Intersect(page2Ids).ShouldBeEmpty();
    }
}
