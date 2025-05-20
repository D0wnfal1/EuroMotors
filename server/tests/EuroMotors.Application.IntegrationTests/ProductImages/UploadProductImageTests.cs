using System.Text;
using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class UploadProductImageTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UploadProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_UploadProductImage_WhenProductExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        Guid categoryId = await ServiceProvider.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await ServiceProvider.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10,
            specifications
        );

        byte[] content = Encoding.UTF8.GetBytes("Mock file content");
        var formFile = new FormFile(
            baseStream: new MemoryStream(content),
            baseStreamOffset: 0,
            length: content.Length,
            name: "image",
            fileName: "test-image.jpg"
        );

        var command = new UploadProductImageCommand(formFile, productId);

        // Act
        ICommandHandler<UploadProductImageCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);

        // Verify the image was saved in the repository
        IProductImageRepository productImageRepository = ServiceProvider.GetRequiredService<IProductImageRepository>();
        ProductImage? savedImage = await productImageRepository.GetByIdAsync(result.Value, CancellationToken.None);

        savedImage.ShouldNotBeNull();
        savedImage.Path.ShouldNotBeNullOrEmpty();
        savedImage.ProductId.ShouldBe(productId);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Create a mock file
        byte[] content = Encoding.UTF8.GetBytes("Mock file content");
        var formFile = new FormFile(
            baseStream: new MemoryStream(content),
            baseStreamOffset: 0,
            length: content.Length,
            name: "image",
            fileName: "test-image.jpg"
        );

        var command = new UploadProductImageCommand(formFile, nonExistentProductId);

        // Act
        ICommandHandler<UploadProductImageCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();

        // Assert
        await Should.ThrowAsync<Exception>(async () =>
        {
            await handler.Handle(command, CancellationToken.None);
        });
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenFileIsEmpty()
    {
        // Arrange
        await CleanDatabaseAsync();

        Guid categoryId = await ServiceProvider.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await ServiceProvider.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10,
            specifications
        );

        // Create an empty file
        var formFile = new FormFile(
            baseStream: new MemoryStream(),
            baseStreamOffset: 0,
            length: 0,
            name: "image",
            fileName: "empty-image.jpg"
        );

        var command = new UploadProductImageCommand(formFile, productId);

        // Act
        ICommandHandler<UploadProductImageCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductImageErrors.InvalidFile(formFile));
    }
}
