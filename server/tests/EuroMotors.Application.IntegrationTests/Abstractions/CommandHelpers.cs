using Bogus;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.ProductImages.UploadProductImage;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Abstractions;

internal static class CommandHelpers
{
    internal static async Task<Guid> CreateUserAsync(this ISender sender)
    {
        var faker = new Faker();
        Result<Guid> result = await sender.Send(
            new RegisterUserCommand(
                faker.Internet.Email(),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Internet.Password()));

        result.IsSuccess.ShouldBeTrue();

        return result.Value;
    }

    public static async Task<Guid> CreateCategoryAsync(this ISender sender, string CategoryName)
    {
        var createCategoryCommand = new CreateCategoryCommand(CategoryName, null, null, null);
        Result<Guid> result = await sender.Send(createCategoryCommand);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateCarBrandAsync(this ISender sender, string brandName, IFormFile? logo = null)
    {
        var createCarBrandCommand = new CreateCarBrandCommand(brandName, logo);
        Result<Guid> result = await sender.Send(createCarBrandCommand);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateCarModelAsync(this ISender sender, Guid brandId, string modelName, int startYear, BodyType bodyType, EngineSpec engineSpec)
    {
        var createCarModelCommand = new CreateCarModelCommand(brandId, modelName, startYear, bodyType, engineSpec);
        Result<Guid> result = await sender.Send(createCarModelCommand);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateCarModelWithBrandAsync(this ISender sender, string brandName, string modelName, int startYear, BodyType bodyType, EngineSpec engineSpec)
    {
        Guid brandId = await sender.CreateCarBrandAsync(brandName);
        return await sender.CreateCarModelAsync(brandId, modelName, startYear, bodyType, engineSpec);
    }

    public static async Task<Guid> CreateProductAsync(
        this ISender sender,
        string productName,
        string ean13,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal discount,
        int quantity,
        List<Specification> specifications)
    {
        var createProductCommand = new CreateProductCommand(
            productName,
            specifications,
            ean13,
            categoryId,
            carModelId,
            price,
            discount,
            quantity
        );

        Result<Guid> result = await sender.Send(createProductCommand);

        result.IsSuccess.ShouldBeTrue();

        return result.Value;
    }

    public static async Task<Guid> CreateProductImageAsync(this ISender sender, Guid productId, IFormFile file)
    {
        var createProductImageCommand = new UploadProductImageCommand(file, productId);
        Result<Guid> result = await sender.Send(createProductImageCommand);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }
}
