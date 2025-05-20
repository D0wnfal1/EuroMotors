using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProductsByBrandName;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class GetProductsByBrandNameTests : BaseIntegrationTest
{
    public GetProductsByBrandNameTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnProducts_WhenBrandHasProducts()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);
        
        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);
        
        for (int i = 0; i < 3; i++)
        {
            var specifications = new List<(string, string)> 
            {
                ($"Color-{i}", $"Value-{i}"), 
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
        
        var query = new GetProductsByBrandNameQuery("Test Brand", "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(3);
        result.Value.Count.ShouldBe(3);
    }
    
    [Fact]
    public async Task Should_ReturnSortedProducts_WhenSortOrderIsProvided()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);
        
        // Create category
        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);
        
        for (int i = 0; i < 3; i++)
        {
            var specifications = new List<(string, string)> 
            {
                ($"Color-{i}", $"Value-{i}"), 
            };
            var product = Product.Create(
                $"Product {i}",
                specifications,
                $"PROD-{i}",
                category.Id,
                new List<CarModel> { carModel },
                100.00m + i * 50,
                0,
                10);
            
            await DbContext.Products.AddAsync(product);
        }
        
        await DbContext.SaveChangesAsync();
        
        var queryDesc = new GetProductsByBrandNameQuery("Test Brand", "DESC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> resultDesc = await handler.Handle(queryDesc, CancellationToken.None);

        // Assert
        resultDesc.IsSuccess.ShouldBeTrue();
        resultDesc.Value.Data.Count.ShouldBe(3);
        
        for (int i = 0; i < resultDesc.Value.Data.Count - 1; i++)
        {
            ProductResponse product1 = resultDesc.Value.Data.ElementAt(i);
            ProductResponse product2 = resultDesc.Value.Data.ElementAt(i + 1);
            product1.Price.ShouldBeGreaterThanOrEqualTo(product2.Price);
        }
        
        var queryAsc = new GetProductsByBrandNameQuery("Test Brand", "ASC", "", 1, 10);
        Result<Pagination<ProductResponse>> resultAsc = await handler.Handle(queryAsc, CancellationToken.None);
        
        for (int i = 0; i < resultAsc.Value.Data.Count - 1; i++)
        {
            ProductResponse product1 = resultAsc.Value.Data.ElementAt(i);
            ProductResponse product2 = resultAsc.Value.Data.ElementAt(i + 1);
            product1.Price.ShouldBeLessThanOrEqualTo(product2.Price);
        }
    }
    
    [Fact]
    public async Task Should_ReturnFilteredProducts_WhenSearchTermIsProvided()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);
        
        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);
        
        var product1 = Product.Create(
            "Special Product",
            new List<(string, string)> { ("Color", "Red") },
            "SP-001",
            category.Id,
            new List<CarModel> { carModel },
            100.00m,
            0,
            10);
            
        var product2 = Product.Create(
            "Regular Product",
            new List<(string, string)> { ("Color", "Blue") },
            "RP-002",
            category.Id,
            new List<CarModel> { carModel },
            150.00m,
            0,
            10);
            
        var product3 = Product.Create(
            "Another Special Item",
            new List<(string, string)> { ("Color", "Green") },
            "SP-003",
            category.Id,
            new List<CarModel> { carModel },
            200.00m,
            0,
            10);
        
        await DbContext.Products.AddRangeAsync(product1, product2, product3);
        await DbContext.SaveChangesAsync();
        
        var query = new GetProductsByBrandNameQuery("Test Brand", "ASC", "Special", 1, 10);

        // Act
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Data.Count.ShouldBe(2); 
        result.Value.Count.ShouldBe(2);
        
        foreach (ProductResponse product in result.Value.Data)
        {
            (product.Name.Contains("Special", StringComparison.OrdinalIgnoreCase) || 
             product.VendorCode.Contains("SP", StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
        }
    }

    [Fact]
    public async Task Should_ReturnPaginatedResults_WhenMultipleProductsExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var engineSpec = new EngineSpec(1.6f, FuelType.Gas);
        var carModel = CarModel.Create(brand, "Test Model", 2020, BodyType.Sedan, engineSpec);
        await DbContext.CarModels.AddAsync(carModel);
        
        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);
        
        for (int i = 0; i < 15; i++)
        {
            var product = Product.Create(
                $"Product {i}",
                new List<(string, string)> { ("Number", i.ToString(System.Globalization.CultureInfo.InvariantCulture)) },
                $"PROD-{i}",
                category.Id,
                new List<CarModel> { carModel },
                100.00m + i,
                0,
                10);
            
            await DbContext.Products.AddAsync(product);
        }
        
        await DbContext.SaveChangesAsync();
        
        var query1 = new GetProductsByBrandNameQuery("Test Brand", "ASC", "", 1, 5);

        // Act
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result1 = await handler.Handle(query1, CancellationToken.None);

        result1.IsSuccess.ShouldBeTrue();
        result1.Value.Data.Count.ShouldBe(5);
        result1.Value.Count.ShouldBe(15); 
        result1.Value.PageIndex.ShouldBe(1);
        
        var query2 = new GetProductsByBrandNameQuery("Test Brand", "ASC", "", 2, 5);
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
    public async Task Should_Fail_WhenBrandDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var query = new GetProductsByBrandNameQuery("NonExistentBrand", "ASC", "", 1, 10);

        // Act
        IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetProductsByBrandNameQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CarModelErrors.BrandNameNotFound("NonExistentBrand"));
    }
} 
