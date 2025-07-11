﻿using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Abstractions;

internal static class CommandHelpers
{
    internal static async Task<Guid> CreateUserAsync(this IServiceProvider serviceProvider)
    {
        var faker = new Faker();
        ICommandHandler<RegisterUserCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        Result<Guid> result = await handler.Handle(
            new RegisterUserCommand(
                faker.Internet.Email(),
                faker.Person.FirstName,
                faker.Person.LastName,
                faker.Internet.Password()),
            CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        return result.Value;
    }

    public static async Task<Guid> CreateCategoryAsync(this IServiceProvider serviceProvider, string CategoryName)
    {
        ICommandHandler<CreateCategoryCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<CreateCategoryCommand, Guid>>();
        var createCategoryCommand = new CreateCategoryCommand(CategoryName, null, null, null);
        Result<Guid> result = await handler.Handle(createCategoryCommand, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateCarBrandAsync(this IServiceProvider serviceProvider, string brandName, IFormFile? logo = null)
    {
        ICommandHandler<CreateCarBrandCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<CreateCarBrandCommand, Guid>>();
        var createCarBrandCommand = new CreateCarBrandCommand(brandName, logo);
        Result<Guid> result = await handler.Handle(createCarBrandCommand, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateCarModelAsync(this IServiceProvider serviceProvider, Guid brandId, string modelName, int startYear, BodyType bodyType, EngineSpec engineSpec)
    {
        ICommandHandler<CreateCarModelCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<CreateCarModelCommand, Guid>>();
        var createCarModelCommand = new CreateCarModelCommand(brandId, modelName, startYear, bodyType, engineSpec);
        Result<Guid> result = await handler.Handle(createCarModelCommand, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }

    public static async Task<Guid> CreateProductAsync(
        this IServiceProvider serviceProvider,
        string productName,
        string ean13,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal discount,
        int quantity,
        List<Specification> specifications)
    {
        ICommandHandler<CreateProductCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<CreateProductCommand, Guid>>();
        var createProductCommand = new CreateProductCommand(
            productName,
            specifications,
            ean13,
            categoryId,
            new List<Guid> { carModelId },
            price,
            discount,
            quantity
        );

        Result<Guid> result = await handler.Handle(createProductCommand, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        return result.Value;
    }

    public static async Task<Guid> CreateOrderAsync(
        this IServiceProvider serviceProvider,
        Guid cartId,
        Guid? userId,
        string buyerName,
        string buyerPhoneNumber,
        string? buyerEmail,
        DeliveryMethod deliveryMethod,
        string? shippingAddress,
        PaymentMethod paymentMethod)
    {
        ICommandHandler<CreateOrderCommand, Guid> handler = serviceProvider.GetRequiredService<ICommandHandler<CreateOrderCommand, Guid>>();
        var createOrderCommand = new CreateOrderCommand(
            cartId,
            userId,
            buyerName,
            buyerPhoneNumber,
            buyerEmail,
            deliveryMethod,
            shippingAddress,
            paymentMethod
        );

        Result<Guid> result = await handler.Handle(createOrderCommand, CancellationToken.None);
        result.IsSuccess.ShouldBeTrue();
        return result.Value;
    }
}
