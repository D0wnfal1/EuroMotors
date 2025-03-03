using System.Data;
using Bogus;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IDbConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();


        SeedCarModels(connection);
        SeedCategories(connection);
        SeedProducts(connection);
        SeedUsers(connection);
        SeedProductImages(connection);
    }

    private static void SeedCarModels(IDbConnection connection)
    {
        Faker<CarModel>? faker = new Faker<CarModel>()
            .CustomInstantiator(f => CarModel.Create(
                f.Vehicle.Manufacturer(),
                f.Vehicle.Model()
            ))
            .RuleFor(c => c.ImageUrl, f => new Uri($"https://loremflickr.com/320/240/car?random={f.Random.Number(1, 1000)}"));

        List<CarModel>? carModels = faker.Generate(10);

        const string sql = @"INSERT INTO car_models (id, brand, model, image_url) 
                         VALUES (@Id, @Brand, @Model, @ImageUrl);";

        connection.Execute(sql, carModels.Select(c => new
        {
            c.Id,
            c.Brand,
            c.Model,
            ImageUrl = c.ImageUrl?.ToString()
        }));
    }

    private static void SeedCategories(IDbConnection connection)
    {
        Faker<Category>? faker = new Faker<Category>()
            .CustomInstantiator(f => Category.Create(
                f.Commerce.Categories(1)[0]
            ))
            .RuleFor(c => c.ImageUrl, f => new Uri($"https://loremflickr.com/320/240/category?random={f.Random.Number(1, 1000)}"));

        List<Category>? categories = faker.Generate(10);

        const string sql = @"INSERT INTO categories (id, name, is_archived, image_url) 
                         VALUES (@Id, @Name, @IsArchived, @ImageUrl);";

        connection.Execute(sql, categories.Select(c => new
        {
            c.Id,
            c.Name,
            c.IsArchived,
            ImageUrl = c.ImageUrl?.ToString()
        }));
    }

    private static void SeedProducts(IDbConnection connection)
    {
        var categories = connection.Query<Category>("SELECT id FROM categories").ToList();

        if (categories.Count == 0)
        {
            return;
        }

        var carModels = connection.Query<CarModel>("SELECT id FROM car_models").ToList();

        if (carModels.Count == 0)
        {
            return;
        }

        Faker<Product> faker = new Faker<Product>()
            .CustomInstantiator(f =>
            {
                Category category = categories[f.Random.Int(0, categories.Count - 1)];
                CarModel carModel = carModels[f.Random.Int(0, carModels.Count - 1)];

                return Product.Create(
                    f.Commerce.ProductName(),
                    f.Lorem.Paragraph(),
                    f.Commerce.Ean13(),
                    category.Id,
                    carModel.Id,
                    f.Random.Decimal(100, 1000),
                    f.Random.Decimal(0, 30),
                    f.Random.Int(0, 100),
                    true
                );
            });

        List<Product> products = faker.Generate(10);

        const string sql = """
                       INSERT INTO products 
                       (id, category_id, car_model_id, name, description, vendor_code, price, discount, stock, is_available)
                        VALUES (@Id, @CategoryId, @CarModelId, @Name, @Description, @VendorCode, @Price, @Discount, @Stock, @IsAvailable);
                       """;

        connection.Execute(sql, products);
    }

    private static void SeedProductImages(IDbConnection connection)
    {
        var products = connection.Query<Product>("SELECT id FROM products").ToList();

        if (products.Count == 0)
        {
            return;
        }

        Faker<ProductImage> faker = new Faker<ProductImage>()
            .CustomInstantiator(f =>
            {
                Product product = products[f.Random.Int(0, products.Count - 1)];

                string? url = f.Image.PicsumUrl();

                return ProductImage.Create(new Uri(url), product.Id);
            });

        const string sql = @"INSERT INTO product_images (id, url, product_id) VALUES (@Id, @Url, @ProductId);";

        var productImages = faker.Generate(15).Select(pi => new
        {
            pi.Id,
            Url = pi.Url.ToString(),
            pi.ProductId
        });

        connection.Execute(sql, productImages);
    }

    private static void SeedUsers(IDbConnection dbConnection)
    {
        Faker<User> faker = new Faker<User>()
            .CustomInstantiator(f => User.Create(
                f.Person.Email,
                f.Person.FirstName,
                f.Person.LastName,
                f.Internet.Password()
            ));

        List<User> users = faker.Generate(10);

        const string sql = @"INSERT INTO users (id, email, first_name, last_name, password_hash) VALUES (@Id, @Email, @FirstName, @LastName, @PasswordHash);";

        dbConnection.Execute(sql, users);
    }
}
