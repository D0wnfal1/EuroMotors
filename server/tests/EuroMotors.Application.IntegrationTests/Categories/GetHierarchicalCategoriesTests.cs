using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Categories.GetHierarchicalCategories;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Categories;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Categories;

public class GetHierarchicalCategoriesTests : BaseIntegrationTest
{
    public GetHierarchicalCategoriesTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var query = new GetHierarchicalCategoriesQuery(1, 10);

        // Act
        IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>>>();
        Result<Pagination<HierarchicalCategoryResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(0);
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_ReturnParentCategories_WithoutChildren()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var parent1 = Category.Create("Parent Category 1");
        var parent2 = Category.Create("Parent Category 2");
        await DbContext.Categories.AddRangeAsync(parent1, parent2);
        await DbContext.SaveChangesAsync();
        
        var query = new GetHierarchicalCategoriesQuery(1, 10);

        // Act
        IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>>>();
        Result<Pagination<HierarchicalCategoryResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(2);
        result.Value.Count.ShouldBe(2);
        
        foreach (HierarchicalCategoryResponse category in result.Value.Data)
        {
            category.SubCategories.ShouldNotBeNull();
            category.SubCategories.ShouldBeEmpty();
        }
    }

    [Fact]
    public async Task Should_ReturnHierarchicalCategories_WithChildren()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var parent1 = Category.Create("Parent Category 1");
        var parent2 = Category.Create("Parent Category 2");
        await DbContext.Categories.AddRangeAsync(parent1, parent2);
        await DbContext.SaveChangesAsync();
        
        var child1 = Category.Create("Child Category 1", parent1.Id);
        var child2 = Category.Create("Child Category 2", parent1.Id);
        var child3 = Category.Create("Child Category 3", parent2.Id);
        await DbContext.Categories.AddRangeAsync(child1, child2, child3);
        await DbContext.SaveChangesAsync();
        
        var query = new GetHierarchicalCategoriesQuery(1, 10);

        // Act
        IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>>>();
        Result<Pagination<HierarchicalCategoryResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(2); 
        result.Value.Count.ShouldBe(2);
        
        HierarchicalCategoryResponse? parentCategory1 = result.Value.Data.FirstOrDefault(c => c.Id == parent1.Id);
        parentCategory1.ShouldNotBeNull();
        parentCategory1.SubCategories.Count.ShouldBe(2);
        
        HierarchicalCategoryResponse? parentCategory2 = result.Value.Data.FirstOrDefault(c => c.Id == parent2.Id);
        parentCategory2.ShouldNotBeNull();
        parentCategory2.SubCategories.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Should_ReturnPaginatedResults_WhenMultipleParentCategoriesExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        for (int i = 0; i < 5; i++)
        {
            var parent = Category.Create($"Parent Category {i}");
            await DbContext.Categories.AddAsync(parent);
        }
        await DbContext.SaveChangesAsync();
        
        var query1 = new GetHierarchicalCategoriesQuery(1, 2);

        // Act - First page
        IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetHierarchicalCategoriesQuery, Pagination<HierarchicalCategoryResponse>>>();
        Result<Pagination<HierarchicalCategoryResponse>> result1 = await handler.Handle(query1, CancellationToken.None);

        // Assert - First page
        result1.IsSuccess.ShouldBeTrue();
        result1.Value.Data.Count.ShouldBe(2);
        result1.Value.Count.ShouldBe(5); 
        result1.Value.PageIndex.ShouldBe(1);
        
        var query2 = new GetHierarchicalCategoriesQuery(2, 2);
        Result<Pagination<HierarchicalCategoryResponse>> result2 = await handler.Handle(query2, CancellationToken.None);
        
        // Assert - Second page
        result2.IsSuccess.ShouldBeTrue();
        result2.Value.Data.Count.ShouldBe(2);
        result2.Value.Count.ShouldBe(5);
        result2.Value.PageIndex.ShouldBe(2);
        
        IEnumerable<Guid> page1Ids = result1.Value.Data.Select(c => c.Id);
        IEnumerable<Guid> page2Ids = result2.Value.Data.Select(c => c.Id);
        page1Ids.Intersect(page2Ids).ShouldBeEmpty();
    }
} 
