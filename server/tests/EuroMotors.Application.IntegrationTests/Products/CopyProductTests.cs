using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.CopyProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class CopyProductTests : BaseIntegrationTest
{
    public CopyProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CopyProduct_Successfully()
    {
        // Arrange
        await CleanDatabaseAsync();

        // Create category
        var category = Category.Create("Test Category");
        await DbContext.Categories.AddAsync(category);

        var brand = CarBrand.Create("Test Brand");
        await DbContext.CarBrands.AddAsync(brand);
        var carModel = CarModel.Create(brand, "Test Model", 2012, BodyType.Convertible, new EngineSpec(1, FuelType.Diesel));
        await DbContext.CarModels.AddAsync(carModel);

        var specifications = new List<(string, string)>
        {
            ("Color", "Red"),
            ("Size", "Large")
        };
        var product = Product.Create(
            "Original Product",
            specifications,
            "ORI-001",
            category.Id,
            new List<CarModel> { carModel },
            100.00m,
            0,
            10);

        await DbContext.Products.AddAsync(product);
        await DbContext.SaveChangesAsync();

        var command = new CopyProductCommand(product.Id);

        // Act
        ICommandHandler<CopyProductCommand, Guid> handler =
            ServiceProvider.GetRequiredService<ICommandHandler<CopyProductCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
        result.Value.ShouldNotBe(product.Id);

        Product? copiedProduct = await DbContext.Products
            .Include(p => p.Specifications)
            .Include(p => p.CarModels)
            .FirstOrDefaultAsync(p => p.Id == result.Value);

        copiedProduct.ShouldNotBeNull();
        copiedProduct.Name.ShouldBe("Original Product (Copy)");
        copiedProduct.VendorCode.ShouldBe(product.VendorCode);
        copiedProduct.VendorCode.ShouldContain("ORI-001");
        copiedProduct.CategoryId.ShouldBe(product.CategoryId);
        copiedProduct.Price.ShouldBe(product.Price);

        copiedProduct.Specifications.Count.ShouldBe(2);

        copiedProduct.CarModels.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Should_Fail_When_SourceProduct_DoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var nonExistentProductId = Guid.NewGuid();
        var command = new CopyProductCommand(nonExistentProductId);

        // Act
        ICommandHandler<CopyProductCommand, Guid> handler =
            ServiceProvider.GetRequiredService<ICommandHandler<CopyProductCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistentProductId));
    }
}
