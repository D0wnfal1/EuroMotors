using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrandById;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarBrands;

public class GetCarBrandByIdTests : BaseIntegrationTest
{
    public GetCarBrandByIdTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnCarBrand_WhenIdExists()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var query = new GetCarBrandByIdQuery(brand.Id);

        // Act
        IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>>();
        Result<CarBrandResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(brand.Id);
        result.Value.Name.ShouldBe("Test Brand");
    }

    [Fact]
    public async Task Should_ReturnError_WhenIdDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var nonExistentId = Guid.NewGuid();
        var query = new GetCarBrandByIdQuery(nonExistentId);

        // Act
        IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>>();
        Result<CarBrandResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CarModelErrors.BrandNotFound(nonExistentId));
    }

    [Fact]
    public async Task Should_ReturnCachedResult_OnSecondCall()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        await DbContext.SaveChangesAsync();
        
        var query = new GetCarBrandByIdQuery(brand.Id);

        // Act - First call
        IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>>();
        Result<CarBrandResponse> firstResult = await handler.Handle(query, CancellationToken.None);
        
        brand.Update("Updated Brand Name");
        await DbContext.SaveChangesAsync();
        
        Result<CarBrandResponse> secondResult = await handler.Handle(query, CancellationToken.None);

        // Assert
        secondResult.IsSuccess.ShouldBeTrue();
        secondResult.Value.Name.ShouldBe("Test Brand");
    }
} 
