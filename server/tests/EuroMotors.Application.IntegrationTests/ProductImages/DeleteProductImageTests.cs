using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.DeleteProductImage;
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

public class DeleteProductImageTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public DeleteProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_DeleteProductImage_WhenImageExists()
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
        byte[] content = Encoding.UTF8.GetBytes("Mock file content");
        var formFile = new FormFile(
            baseStream: new MemoryStream(content),
            baseStreamOffset: 0,
            length: content.Length,
            name: "image",
            fileName: "test-image.jpg"
        );

        var uploadCommand = new UploadProductImageCommand(formFile, productId);
        ICommandHandler<UploadProductImageCommand, Guid> uploadHandler = ServiceProvider.GetRequiredService<ICommandHandler<UploadProductImageCommand, Guid>>();
        Result<Guid> uploadResult = await uploadHandler.Handle(uploadCommand, CancellationToken.None);
        
        uploadResult.IsSuccess.ShouldBeTrue();
        Guid imageId = uploadResult.Value;

        // Get image to verify it exists
        IProductImageRepository productImageRepository = ServiceProvider.GetRequiredService<IProductImageRepository>();
        ProductImage? uploadedImage = await productImageRepository.GetByIdAsync(imageId, CancellationToken.None);
        uploadedImage.ShouldNotBeNull();

        // Create delete command
        var deleteCommand = new DeleteProductImageCommand(imageId);

        // Act
        ICommandHandler<DeleteProductImageCommand> deleteHandler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteProductImageCommand>>();
        Result deleteResult = await deleteHandler.Handle(deleteCommand, CancellationToken.None);

        // Assert
        deleteResult.IsSuccess.ShouldBeTrue();
        
        // Verify image is deleted
        ProductImage? deletedImage = await productImageRepository.GetByIdAsync(imageId, CancellationToken.None);
        deletedImage.ShouldBeNull();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenImageDoesNotExist()
    {
        // Arrange
        var nonExistentImageId = Guid.NewGuid();
        var command = new DeleteProductImageCommand(nonExistentImageId);

        // Act
        ICommandHandler<DeleteProductImageCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteProductImageCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductImageErrors.ProductImageNotFound(nonExistentImageId));
    }
} 
