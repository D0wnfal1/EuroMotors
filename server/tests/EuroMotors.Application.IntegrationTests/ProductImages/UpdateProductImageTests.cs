using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Text;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class UpdateProductImageTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UpdateProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateProductImage_WhenImageExists()
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

        // Upload an image first
        byte[] initialContent = Encoding.UTF8.GetBytes("Initial mock content");
        var initialFile = new FormFile(
            baseStream: new MemoryStream(initialContent),
            baseStreamOffset: 0,
            length: initialContent.Length,
            name: "image",
            fileName: "initial-image.jpg"
        );

        var uploadCommand = new UploadProductImageCommand(initialFile, productId);
        ICommandHandler<UploadProductImageCommand, Guid> uploadHandler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();
        Result<Guid> uploadResult = await uploadHandler.Handle(uploadCommand, CancellationToken.None);
        
        uploadResult.IsSuccess.ShouldBeTrue();
        Guid imageId = uploadResult.Value;

        // Get the original image path
        IProductImageRepository productImageRepository = ServiceProvider.GetRequiredService<IProductImageRepository>();
        ProductImage? originalImage = await productImageRepository.GetByIdAsync(imageId, CancellationToken.None);
        originalImage.ShouldNotBeNull();
        string originalPath = originalImage.Path;

        // Create new image content for update
        byte[] updatedContent = Encoding.UTF8.GetBytes("Updated mock content");
        var updatedFile = new FormFile(
            baseStream: new MemoryStream(updatedContent),
            baseStreamOffset: 0,
            length: updatedContent.Length,
            name: "image",
            fileName: "updated-image.jpg"
        );

        var updateCommand = new UpdateProductImageCommand(imageId, updatedFile, productId);

        // Act
        ICommandHandler<UpdateProductImageCommand> updateHandler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductImageCommand>>();
        Result updateResult = await updateHandler.Handle(updateCommand, CancellationToken.None);

        // Assert
        updateResult.IsSuccess.ShouldBeTrue();
        
        // Verify image is updated
        ProductImage? updatedImage = await productImageRepository.GetByIdAsync(imageId, CancellationToken.None);
        updatedImage.ShouldNotBeNull();
        updatedImage.Path.ShouldNotBeNullOrEmpty();
        updatedImage.Path.ShouldNotBe(originalPath); // Path should be different after update
        updatedImage.ProductId.ShouldBe(productId);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenImageDoesNotExist()
    {
        // Arrange
        var nonExistentImageId = Guid.NewGuid();
        byte[] content = Encoding.UTF8.GetBytes("Mock content");
        var formFile = new FormFile(
            baseStream: new MemoryStream(content),
            baseStreamOffset: 0,
            length: content.Length,
            name: "image",
            fileName: "test-image.jpg"
        );

        var command = new UpdateProductImageCommand(nonExistentImageId, formFile, Guid.NewGuid());

        // Act
        ICommandHandler<UpdateProductImageCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductImageCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductImageErrors.ProductImageNotFound(nonExistentImageId));
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

        // Upload an image first
        byte[] initialContent = Encoding.UTF8.GetBytes("Initial mock content");
        var initialFile = new FormFile(
            baseStream: new MemoryStream(initialContent),
            baseStreamOffset: 0,
            length: initialContent.Length,
            name: "image",
            fileName: "initial-image.jpg"
        );

        var uploadCommand = new UploadProductImageCommand(initialFile, productId);
        ICommandHandler<UploadProductImageCommand, Guid> uploadHandler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();
        Result<Guid> uploadResult = await uploadHandler.Handle(uploadCommand, CancellationToken.None);
        
        uploadResult.IsSuccess.ShouldBeTrue();
        Guid imageId = uploadResult.Value;

        // Create an empty file for update
        var emptyFile = new FormFile(
            baseStream: new MemoryStream(),
            baseStreamOffset: 0,
            length: 0,
            name: "image",
            fileName: "empty-image.jpg"
        );

        var updateCommand = new UpdateProductImageCommand(imageId, emptyFile, productId);

        // Act
        ICommandHandler<UpdateProductImageCommand> updateHandler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductImageCommand>>();
        Result updateResult = await updateHandler.Handle(updateCommand, CancellationToken.None);

        // Assert
        updateResult.IsSuccess.ShouldBeFalse();
        updateResult.Error.ShouldNotBeNull();
        updateResult.Error.ShouldBe(ProductImageErrors.InvalidFile(emptyFile));
    }
} 
