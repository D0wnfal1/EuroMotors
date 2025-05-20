using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarModels.GetCarModelSelection;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class GetCarModelSelectionTests : BaseIntegrationTest
{
    public GetCarModelSelectionTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }


    [Fact]
    public async Task Should_ReturnAllOptions_WhenModelsExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand1 = CarBrand.Create("BMW");
        var brand2 = CarBrand.Create("Audi");
        await DbContext.CarBrands.AddRangeAsync(brand1, brand2);
        await DbContext.SaveChangesAsync();
        
        var engineSpec1 = new EngineSpec(2.0f, FuelType.Petrol);
        var engineSpec2 = new EngineSpec(3.0f, FuelType.Diesel);
        var engineSpec3 = new EngineSpec(1.8f, FuelType.Petrol);
        
        var model1 = CarModel.Create(brand1, "X5", 2020, BodyType.SUV, engineSpec1);
        var model2 = CarModel.Create(brand1, "X3", 2021, BodyType.SUV, engineSpec2);
        var model3 = CarModel.Create(brand2, "A4", 2019, BodyType.Sedan, engineSpec3);
        
        await DbContext.CarModels.AddRangeAsync(model1, model2, model3);
        await DbContext.SaveChangesAsync();
        
        var query = new GetCarModelSelectionQuery(null, null, null, null, null);

        // Act
        IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse>>();
        Result<CarModelSelectionResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Ids.Count.ShouldBe(3);
        result.Value.Brands.Count.ShouldBe(2);
        result.Value.Models.Count.ShouldBe(3);
        result.Value.Years.Count.ShouldBe(3);
        result.Value.BodyTypes.Count.ShouldBe(2);
        result.Value.EngineSpecs.Count.ShouldBe(3);
        
        result.Value.Brands.ShouldContain(b => b.Name == "BMW");
        result.Value.Brands.ShouldContain(b => b.Name == "Audi");
        
        result.Value.Models.ShouldContain("X5");
        result.Value.Models.ShouldContain("X3");
        result.Value.Models.ShouldContain("A4");
        
        result.Value.BodyTypes.ShouldContain("SUV");
        result.Value.BodyTypes.ShouldContain("Sedan");
    }

    [Fact]
    public async Task Should_ReturnFilteredOptions_WhenBrandSpecified()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var brand1 = CarBrand.Create("BMW");
        var brand2 = CarBrand.Create("Audi");
        await DbContext.CarBrands.AddRangeAsync(brand1, brand2);
        await DbContext.SaveChangesAsync();
        
        var engineSpec1 = new EngineSpec(2.0f, FuelType.Petrol);
        var engineSpec2 = new EngineSpec(3.0f, FuelType.Diesel);
        var engineSpec3 = new EngineSpec(1.8f, FuelType.Petrol);
        
        var model1 = CarModel.Create(brand1, "X5", 2020, BodyType.SUV, engineSpec1);
        var model2 = CarModel.Create(brand1, "X3", 2021, BodyType.SUV, engineSpec2);
        var model3 = CarModel.Create(brand2, "A4", 2019, BodyType.Sedan, engineSpec3);
        
        await DbContext.CarModels.AddRangeAsync(model1, model2, model3);
        await DbContext.SaveChangesAsync();
        
        var query = new GetCarModelSelectionQuery(brand1.Id, null, null, null, null);

        // Act
        IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse> handler = 
            ServiceProvider.GetRequiredService<IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse>>();
        Result<CarModelSelectionResponse> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Ids.Count.ShouldBe(2); 
        result.Value.Models.Count.ShouldBe(2); 
        
        result.Value.Models.ShouldContain("X5");
        result.Value.Models.ShouldContain("X3");
        result.Value.Models.ShouldNotContain("A4");
        
        result.Value.BodyTypes.Count.ShouldBe(1);
        result.Value.BodyTypes.ShouldContain("SUV");
        result.Value.BodyTypes.ShouldNotContain("Sedan");
    }

} 
