using System.Data;
using Bogus;
using Dapper;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Domain.CarBrands;
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

        SeedCarBrands(connection);
        SeedCarModels(connection);
        SeedCategories(connection);
        SeedProducts(connection);
        SeedUsers(connection, passwordHasher);
    }

    private static void SeedCarBrands(IDbConnection connection)
    {
        int brandsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM car_brands");
        if (brandsCount > 0)
        {
            return;
        }

        string[] carBrands =
        [
            "BMW", "Mercedes-Benz", "Audi", "Toyota", "Honda",
            "Ford", "Chevrolet", "Volkswagen", "Hyundai", "Kia",
            "Volvo", "Tesla", "Porsche", "Nissan", "Mazda"
        ];

        var brands = carBrands.Select(name => CarBrand.Create(name)).ToList();

        const string sql = @"INSERT INTO car_brands (id, name, slug, logo_path) 
                       VALUES (@Id, @Name, @Slug, @LogoPath);";

        connection.Execute(sql, brands.Select(b => new
        {
            b.Id,
            b.Name,
            Slug = b.Slug.Value,
            b.LogoPath
        }));
    }

    private static void SeedCarModels(IDbConnection connection)
    {
        int modelsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM car_models");
        if (modelsCount > 0)
        {
            return;
        }

        var brands = connection.Query<(Guid Id, string Name)>(
            "SELECT id, name FROM car_brands").ToList();

        if (brands.Count == 0)
        {
            return;
        }

        var carModels = new List<CarModel>();

        var brandModels = new Dictionary<string, string[]>
        {
            ["BMW"] = ["1 Series", "2 Series", "3 Series", "4 Series", "5 Series", "7 Series", "X1", "X3", "X5", "M3", "M5"],
            ["Mercedes-Benz"] = ["A-Class", "C-Class", "E-Class", "S-Class", "GLA", "GLC", "GLE", "AMG GT"],
            ["Audi"] = ["A3", "A4", "A6", "Q3", "Q5", "Q7", "TT", "R8", "RS6"],
            ["Toyota"] = ["Corolla", "Camry", "RAV4", "Highlander", "Prius", "Land Cruiser", "Yaris"],
            ["Honda"] = ["Civic", "Accord", "CR-V", "Pilot", "Fit", "HR-V", "Odyssey"],
            ["Ford"] = ["F-150", "Mustang", "Explorer", "Escape", "Focus", "Edge", "Ranger"],
            ["Chevrolet"] = ["Silverado", "Malibu", "Equinox", "Tahoe", "Camaro", "Bolt", "Corvette"],
            ["Volkswagen"] = ["Golf", "Passat", "Tiguan", "Atlas", "Jetta", "Arteon", "ID.4"],
            ["Hyundai"] = ["Elantra", "Sonata", "Tucson", "Santa Fe", "Kona", "Palisade", "Ioniq"],
            ["Kia"] = ["Forte", "Optima", "Sportage", "Sorento", "Soul", "Telluride", "Stinger"],
            ["Volvo"] = ["S60", "S90", "XC40", "XC60", "XC90", "V60", "V90"],
            ["Tesla"] = ["Model 3", "Model S", "Model X", "Model Y", "Cybertruck"],
            ["Porsche"] = ["911", "Cayenne", "Macan", "Panamera", "Taycan", "718 Cayman", "718 Boxster"],
            ["Nissan"] = ["Altima", "Maxima", "Rogue", "Pathfinder", "Sentra", "Murano", "Leaf"],
            ["Mazda"] = ["Mazda3", "Mazda6", "CX-5", "CX-9", "MX-5 Miata", "CX-30"]
        };

        var faker = new Faker();

        foreach ((Guid brandId, string brandName) in brands)
        {
            if (!brandModels.TryGetValue(brandName, out string[]? models))
            {
                continue;
            }

            foreach (string modelName in models)
            {
                var brand = CarBrand.Create(brandName);
                brand.Id = brandId;

                var carModel = CarModel.Create(
                    brand,
                    modelName,
                    faker.Date.Past(20).Year,
                    faker.PickRandom<BodyType>(),
                    new EngineSpec(MathF.Round(faker.Random.Float(1, 5), 1), faker.PickRandom<FuelType>())
                );

                carModels.Add(carModel);
            }
        }

        const string sql = @"INSERT INTO car_models (id, car_brand_id, model_name, start_year, body_type, engine_spec_volume_liters, engine_spec_fuel_type, slug) 
                         VALUES (@Id, @CarBrandId, @ModelName, @StartYear, @BodyType, @VolumeLiters, @FuelType, @Slug);";

        connection.Execute(sql, carModels.Select(c => new
        {
            c.Id,
            c.CarBrandId,
            c.ModelName,
            c.StartYear,
            BodyType = c.BodyType.ToString(),
            c.EngineSpec.VolumeLiters,
            FuelType = c.EngineSpec.FuelType.ToString(),
            Slug = c.Slug.Value
        }));
    }

    private static void SeedCategories(IDbConnection connection)
    {
        var random = new Random();
        var allCategories = new List<Category>();

        var parentCategories = new List<(string Name, List<string> Subcategories)>
    {
        ("Electronics",
        [
            "Smartphones", "Laptops", "Tablets", "Cameras", "Drones", "Smartwatches", "Headphones", "Chargers",
            "Power Banks", "TVs", "Monitors", "Speakers", "VR Headsets"
        ]),
        ("Fashion",
        [
            "Men's Clothing", "Women's Clothing", "Kids' Clothing", "Shoes", "Bags", "Accessories", "Hats", "Watches",
            "Jewelry", "Sunglasses", "Belts", "Scarves", "Underwear"
        ]),
        ("Home & Kitchen",
        [
            "Furniture", "Bedding", "Cookware", "Small Appliances", "Cleaning Supplies", "Lighting", "Decor", "Tools",
            "Storage", "Vacuum Cleaners", "Air Purifiers", "Curtains"
        ]),
        ("Sports & Outdoors",
        [
            "Fitness Equipment", "Bicycles", "Camping", "Hiking", "Running", "Swimming", "Yoga", "Fishing",
            "Winter Sports", "Balls", "Backpacks", "Tents", "Water Bottles"
        ]),
        ("Toys & Games",
        [
            "Educational Toys", "Board Games", "Puzzles", "Building Sets", "Remote Control Toys", "Stuffed Animals",
            "Dolls", "Outdoor Toys", "Musical Toys", "Action Figures", "LEGO", "Toy Vehicles"
        ]),
        ("Beauty & Health",
        [
            "Makeup", "Skincare", "Haircare", "Perfume", "Manicure", "Personal Hygiene", "Supplements", "Massagers",
            "Toothbrushes", "Hair Dryers", "Razors", "Body Wash"
        ]),
        ("Automotive",
        [
            "Car Electronics", "Car Care", "Tires", "Motor Oils", "Interior Accessories", "Exterior Accessories",
            "Tools", "Batteries", "Car Seats", "GPS Navigators", "Dash Cams", "Helmets"
        ]),
        ("Books",
        [
            "Fiction", "Non-Fiction", "Children's Books", "Comics", "Educational", "Science", "History", "Biography",
            "Self-Help", "Mystery", "Fantasy", "Business", "Technology"
        ]),
        ("Groceries",
        [
            "Fruits", "Vegetables", "Dairy", "Meat", "Snacks", "Beverages", "Bakery", "Canned Goods", "Frozen Food",
            "Pasta", "Sauces", "Grains", "Tea & Coffee"
        ]),
        ("Pets",
        [
            "Dog Food", "Cat Food", "Bird Supplies", "Aquarium", "Pet Toys", "Beds", "Leashes", "Bowls", "Litter",
            "Pet Grooming", "Treats", "Cages", "Health Products"
        ])
    };

        foreach ((string name, List<string> subcategories) in parentCategories)
        {
            var parent = Category.Create(name);
            allCategories.Add(parent);

#pragma warning disable CA5394
            int subCount = random.Next(10, Math.Min(subcategories.Count, 30));
#pragma warning restore CA5394
            foreach (string subName in subcategories.Take(subCount))
            {
                var subCategory = Category.Create(subName, parent.Id);
                parent.AddSubcategory(subCategory);
                allCategories.Add(subCategory);
            }
        }

        const string sql = @"INSERT INTO categories (id, name, is_available, image_path, parent_category_id, slug) 
                         VALUES (@Id, @Name, @IsAvailable, @ImagePath, @ParentCategoryId, @Slug);";

        connection.Execute(sql, allCategories.Select(c => new
        {
            c.Id,
            c.Name,
            c.IsAvailable,
            c.ImagePath,
            c.ParentCategoryId,
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

        Faker<Product>? faker = new Faker<Product>()
            .CustomInstantiator(f =>
            {
                Category? category = f.PickRandom(categories);
                CarModel? carModel = f.PickRandom(carModels);

                var product = Product.Create(
                    f.Commerce.ProductName(),
                    null,
                    f.Commerce.Ean13(),
                    category.Id,
                    carModel.Id,
                    f.Random.Decimal(100, 1000),
                    f.Random.Decimal(0, 30),
                    f.Random.Int(0, 100)
                );

                var specFaker = new Faker();
                int specsCount = f.Random.Int(4, 10);

                var uniqueSpecNames = new HashSet<string>();

                for (int i = 0; i < specsCount; i++)
                {
                    string specName;
                    do
                    {
                        specName = specFaker.Commerce.ProductMaterial();
                    }
                    while (!uniqueSpecNames.Add(specName));

                    string specValue = specFaker.Random.Word();
                    product.AddSpecification(specName, specValue);
                }

                return product;
            });

        List<Product>? products = faker.Generate(500);

        const string insertProductsSql = """
        INSERT INTO products 
        (id, category_id, car_model_id, name, vendor_code, price, discount, stock, is_available, slug)
        VALUES (@Id, @CategoryId, @CarModelId, @Name, @VendorCode, @Price, @Discount, @Stock, @IsAvailable, @Slug);
    """;

        const string insertSpecificationsSql = """
        INSERT INTO product_specifications
        (product_id, specification_name, specification_value)
        VALUES (@ProductId, @SpecificationName, @SpecificationValue);
    """;

        connection.Execute(insertProductsSql, products.Select(p => new
        {
            p.Id,
            p.CategoryId,
            p.CarModelId,
            p.Name,
            p.VendorCode,
            p.Price,
            p.Discount,
            p.Stock,
            p.IsAvailable,
            Slug = p.Slug.Value
        }));

        var allSpecifications = products
            .SelectMany(p => p.Specifications.Select(s => new
            {
                ProductId = p.Id,
                s.SpecificationName,
                s.SpecificationValue
            }))
            .ToList();

        connection.Execute(insertSpecificationsSql, allSpecifications);
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
