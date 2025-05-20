using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProductsByCategoryWithChildren;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class GetProductsByCategoryWithChildrenTests : BaseIntegrationTest
{
    public GetProductsByCategoryWithChildrenTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenCategoryHasNoProducts()
    {
        // Arrange
        await CleanDatabaseAsync();

        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);
        await DbContext.SaveChangesAsync();

        var query = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(0);
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_ReturnProducts_WhenCategoryHasProducts()
    {
        // Arrange
        await CleanDatabaseAsync();

        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();

        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);

        for (int i = 0; i < 3; i++)
        {
            var specifications = new List<(string, string)>
            {
                ($"Feature-{i}", $"Value-{i}"),
            };
            var product = Product.Create(
                $"Product {i}",
                specifications,
                $"PROD-{i}",
                category.Id,
                new List<CarModel> { carModel },
                100.00m + i * 10,
                0,
                10);

            await DbContext.Products.AddAsync(product);
        }

        await DbContext.SaveChangesAsync();

        var query = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(3);
        result.Value.Count.ShouldBe(3);

        foreach (ProductResponse product in result.Value.Data)
        {
            product.CategoryId.ShouldBe(category.Id);
            product.Images.ShouldNotBeNull();
            product.Specifications.ShouldNotBeNull();
            product.CarModelIds.ShouldNotBeNull();
            product.CarModelIds.ShouldContain(carModel.Id);
        }
    }

    [Fact]
    public async Task Should_ReturnProductsFromChildCategories()
    {
        // Arrange
        await CleanDatabaseAsync();

        var parentCategory = Category.Create("Parent Category");
        await DbContext.Categories.AddAsync(parentCategory);
        await DbContext.SaveChangesAsync();

        var childCategory = Category.Create("Child Category", parentCategory.Id);
        await DbContext.Categories.AddAsync(childCategory);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();

        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);

        for (int i = 0; i < 2; i++)
        {
            var product = Product.Create(
                $"Child Product {i}",
                new List<(string, string)> { ($"Feature-{i}", $"Value-{i}") },
                $"CHILD-{i}",
                childCategory.Id,
                new List<CarModel> { carModel },
                100.00m + i,
                0,
                10);

            await DbContext.Products.AddAsync(product);
        }

        var parentProduct = Product.Create(
            "Parent Product",
            new List<(string, string)> { ("Feature", "Value") },
            "PARENT-1",
            parentCategory.Id,
            new List<CarModel> { carModel },
            200.00m,
            0,
            10);

        await DbContext.Products.AddAsync(parentProduct);
        await DbContext.SaveChangesAsync();

        var query = new GetProductsByCategoryWithChildrenQuery(parentCategory.Id, "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(3);
        result.Value.Count.ShouldBe(3);

        result.Value.Data.Count(p => p.CategoryId == parentCategory.Id).ShouldBe(1);

        result.Value.Data.Count(p => p.CategoryId == childCategory.Id).ShouldBe(2);
    }

    [Fact]
    public async Task Should_ReturnSortedProducts_WhenSortOrderIsProvided()
    {
        // Arrange
        await CleanDatabaseAsync();

        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();

        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);

        for (int i = 0; i < 3; i++)
        {
            var product = Product.Create(
                $"Product {i}",
                new List<(string, string)> { ("Feature", "Value") },
                $"PROD-{i}",
                category.Id,
                new List<CarModel> { carModel },
                100.00m + i * 50,
                0,
                10);

            await DbContext.Products.AddAsync(product);
        }

        await DbContext.SaveChangesAsync();

        var queryDesc = new GetProductsByCategoryWithChildrenQuery(category.Id, "DESC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> resultDesc = await handler.Handle(queryDesc, CancellationToken.None);

        // Assert
        resultDesc.IsSuccess.ShouldBeTrue();
        resultDesc.Value.Data.Count.ShouldBe(3);

        var queryAsc = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "", 1, 10);
        Result<Pagination<ProductResponse>> resultAsc = await handler.Handle(queryAsc, CancellationToken.None);
    }

    [Fact]
    public async Task Should_ReturnFilteredProducts_WhenSearchTermIsProvided()
    {
        // Arrange
        await CleanDatabaseAsync();

        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();

        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);

        var product1 = Product.Create(
            "Premium Product",
            new List<(string, string)> { ("Feature", "Value") },
            "PP-001",
            category.Id,
            new List<CarModel> { carModel },
            100.00m,
            0,
            10);

        var product2 = Product.Create(
            "Standard Product",
            new List<(string, string)> { ("Feature", "Value") },
            "SP-002",
            category.Id,
            new List<CarModel> { carModel },
            150.00m,
            0,
            10);

        var product3 = Product.Create(
            "Another Premium Item",
            new List<(string, string)> { ("Feature", "Value") },
            "AP-003",
            category.Id,
            new List<CarModel> { carModel },
            200.00m,
            0,
            10);

        await DbContext.Products.AddRangeAsync(product1, product2, product3);
        await DbContext.SaveChangesAsync();

        var query = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "Premium", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(2);
        result.Value.Count.ShouldBe(2);

        foreach (ProductResponse product in result.Value.Data)
        {
            product.Name.Contains("Premium", StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task Should_ReturnPaginatedResults_WhenMultipleProductsExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();

        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);

        for (int i = 0; i < 15; i++)
        {
            var product = Product.Create(
                $"Product {i}",
                new List<(string, string)> { ("Feature", "Value") },
                $"PROD-{i}",
                category.Id,
                new List<CarModel> { carModel },
                100.00m + i,
                0,
                10);

            await DbContext.Products.AddAsync(product);
        }

        await DbContext.SaveChangesAsync();

        var query1 = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "", 1, 5);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result1 = await handler.Handle(query1, CancellationToken.None);

        // Assert first page
        result1.IsSuccess.ShouldBeTrue();
        result1.Value.Data.Count.ShouldBe(5);
        result1.Value.Count.ShouldBe(15);
        result1.Value.PageIndex.ShouldBe(1);

        var query2 = new GetProductsByCategoryWithChildrenQuery(category.Id, "ASC", "", 2, 5);
        Result<Pagination<ProductResponse>> result2 = await handler.Handle(query2, CancellationToken.None);

        result2.IsSuccess.ShouldBeTrue();
        result2.Value.Data.Count.ShouldBe(5);
        result2.Value.Count.ShouldBe(15);
        result2.Value.PageIndex.ShouldBe(2);

        result1.Value.Data.Select(p => p.Id)
            .Intersect(result2.Value.Data.Select(p => p.Id))
            .ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_Fail_WhenCategoryDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var nonExistentCategoryId = Guid.NewGuid();
        var query = new GetProductsByCategoryWithChildrenQuery(nonExistentCategoryId, "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByCategoryWithChildrenQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NotFound(nonExistentCategoryId));
    }
}
