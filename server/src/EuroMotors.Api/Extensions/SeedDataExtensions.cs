using System.Data;
using Bogus;
using Dapper;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;

namespace EuroMotors.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app, IPasswordHasher passwordHasher)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IDbConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();


        SeedCarModels(connection);
        SeedCategories(connection);
        SeedProducts(connection);
        SeedUsers(connection, passwordHasher);
    }

    private static void SeedCarModels(IDbConnection connection)
    {
        Faker<CarModel>? faker = new Faker<CarModel>()
            .CustomInstantiator(f =>
                CarModel.Create(
                    f.Vehicle.Manufacturer(),
                    f.Vehicle.Model(),
                    f.Date.Past(20).Year,
                    f.Random.Bool() ? (int?)f.Date.Past(10).Year : null,
                    f.PickRandom<BodyType>(),
                    new EngineSpec(f.Random.Number(1, 5), f.PickRandom<FuelType>(), f.Random.Number(100, 500))
                )
            );

        List<CarModel>? carModels = faker.Generate(10);


        const string sql = @"INSERT INTO car_models (id, brand, model, start_year, end_year, body_type, engine_spec_volume_liters, engine_spec_fuel_type, engine_spec_horse_power, slug, image_path) 
                         VALUES (@Id, @Brand, @Model, @StartYear, @EndYear, @BodyType, @VolumeLiters, @FuelType, @HorsePower, @Slug, @ImagePath);";

        connection.Execute(sql, carModels.Select(c => new
        {
            c.Id,
            c.Brand,
            c.Model,
            c.StartYear,
            c.EndYear,
            BodyType = c.BodyType.ToString(),
            c.EngineSpec.VolumeLiters,
            FuelType = c.EngineSpec.FuelType.ToString(),
            c.EngineSpec.HorsePower,
            Slug = c.Slug.Value,
            c.ImagePath
        }));
    }

    private static void SeedCategories(IDbConnection connection)
    {
        var uniqueCategories = new HashSet<string>();

        Faker<Category> faker = new Faker<Category>()
            .CustomInstantiator(f =>
            {
                string? categoryName = f.Commerce.Categories(1)[0];

                while (!uniqueCategories.Add(categoryName))
                {
                    categoryName = f.Commerce.Categories(1)[0];
                }

                return Category.Create(categoryName);
            });

        List<Category>? categories = faker.Generate(10);

        const string sql = @"INSERT INTO categories (id, name, is_archived, image_path, slug) 
                         VALUES (@Id, @Name, @IsArchived, @ImagePath, @Slug);";

        connection.Execute(sql, categories.Select(c => new
        {
            c.Id,
            c.Name,
            c.IsArchived,
            c.ImagePath,
            Slug = c.Slug.Value
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
                    f.Random.Int(0, 100)
                );
            });

        List<Product> products = faker.Generate(10);

        const string sql = """
                       INSERT INTO products 
                       (id, category_id, car_model_id, name, description, vendor_code, price, discount, stock, is_available, slug)
                        VALUES (@Id, @CategoryId, @CarModelId, @Name, @Description, @VendorCode, @Price, @Discount, @Stock, @IsAvailable, @Slug);
                       """;

        connection.Execute(sql, products.Select(p => new
        {
            p.Id,
            p.CategoryId,
            p.CarModelId,
            p.Name,
            p.Description,
            p.VendorCode,
            p.Price,
            p.Discount,
            p.Stock,
            p.IsAvailable,
            Slug = p.Slug.Value
        }));
    }

    private static void SeedUsers(IDbConnection connection, IPasswordHasher passwordHasher)
    {
        var existingUsers = connection.Query<Guid>("SELECT id FROM users").ToList();
        if (existingUsers.Count > 0)
        {
            return;
        }

        var roles = new[]
        {
            new { Id = 1, Name = "Admin" },
            new { Id = 2, Name = "Customer" }
        };

        User[] users =
        [
            User.Create("admin@example.com", "Admin", "User", passwordHasher.Hash("Admin123!")),
             User.Create("customer@example.com", "Customer", "User", passwordHasher.Hash("Customer123!"))
        ];

        const string userSql = @"
        INSERT INTO users (id, email, first_name, last_name, password_hash) 
        VALUES (@Id, @Email, @FirstName, @LastName, @PasswordHash);
    ";
        connection.Execute(userSql, users);

        var roleUserMappings = new[]
        {
            new { UserId = users[0].Id, RoleId = roles[0].Id },
            new { UserId = users[1].Id, RoleId = roles[1].Id }
        };

        const string roleUserSql = @"
        INSERT INTO role_user (roles_id, users_id) 
        VALUES (@RoleId, @UserId);
    ";
        connection.Execute(roleUserSql, roleUserMappings);
    }
}
