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
            "Volvo", "Tesla", "Porsche", "Nissan", "Mazda",
            "Lexus", "Subaru", "Jeep", "Dodge", "Fiat",
            "Alfa Romeo", "Jaguar", "Land Rover", "Bentley", "Bugatti",
            "Ferrari", "Lamborghini", "Maserati", "Rolls-Royce", "Aston Martin",
            "McLaren", "Mini", "Cadillac", "Chrysler", "GMC",
            "Acura", "Infiniti", "Genesis", "Buick", "Lincoln",
            "RAM", "Mitsubishi", "Peugeot", "Renault", "Citroen",
            "Skoda", "Seat", "Opel", "Vauxhall", "Dacia"
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
        int categoriesCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM categories");
        if (categoriesCount > 0)
        {
            return;
        }

        var random = new Random();
        var allCategories = new List<Category>();

        var parentCategories = new List<(string Name, List<string> Subcategories)>
        {
            ("Engine Parts",
            [
                "Pistons", "Crankshafts", "Camshafts", "Engine Blocks", "Cylinder Heads", "Gaskets", "Valves",
                "Valve Springs", "Timing Chains", "Timing Belts", "Oil Pumps", "Water Pumps", "Connecting Rods",
                "Bearings", "Engine Mounts", "Turbochargers", "Superchargers", "Intercoolers", "Engine Sensors"
            ]),
            ("Transmission",
            [
                "Gearboxes", "Clutches", "Torque Converters", "Transmission Fluid", "Shift Kits", "Gear Sets",
                "Transmission Mounts", "Transmission Coolers", "Transmission Filters", "Automatic Transmissions",
                "Manual Transmissions", "Dual-Clutch Transmissions", "CVT Transmissions", "Transmission Sensors"
            ]),
            ("Braking System",
            [
                "Brake Pads", "Brake Rotors", "Brake Calipers", "Brake Shoes", "Brake Drums", "Brake Lines",
                "Master Cylinders", "Brake Boosters", "ABS Sensors", "Brake Fluid", "Brake Hoses",
                "Brake Pad Wear Sensors", "Parking Brake Cables", "Brake Light Switches"
            ]),
            ("Suspension & Steering",
            [
                "Shock Absorbers", "Struts", "Coil Springs", "Leaf Springs", "Control Arms", "Ball Joints",
                "Tie Rods", "Steering Racks", "Power Steering Pumps", "Sway Bars", "Bushings", "Strut Mounts",
                "Wheel Bearings", "Hub Assemblies", "Steering Wheels", "Steering Columns", "Alignment Kits"
            ]),
            ("Electrical",
            [
                "Batteries", "Alternators", "Starters", "Ignition Coils", "Spark Plugs", "Spark Plug Wires",
                "Fuses", "Relays", "ECUs", "Wiring Harnesses", "Sensors", "Switches", "Battery Cables",
                "Headlights", "Taillights", "Turn Signals", "Bulbs", "LED Lighting", "Car Audio", "Alarms"
            ]),
            ("Cooling System",
            [
                "Radiators", "Cooling Fans", "Thermostats", "Coolant", "Expansion Tanks", "Radiator Caps",
                "Hoses", "Water Pumps", "Heater Cores", "Fan Belts", "Fan Clutches", "Temperature Sensors",
                "Radiator Shrouds", "Cooling System Additives"
            ]),
            ("Fuel System",
            [
                "Fuel Pumps", "Fuel Injectors", "Fuel Filters", "Fuel Tanks", "Fuel Lines", "Carburetors",
                "Fuel Pressure Regulators", "Throttle Bodies", "Fuel Rails", "Fuel Caps", "Intake Manifolds",
                "Air Filters", "Mass Air Flow Sensors", "Oxygen Sensors", "EGR Valves", "Fuel Gauges"
            ]),
            ("Exhaust System",
            [
                "Exhaust Manifolds", "Catalytic Converters", "Mufflers", "Exhaust Pipes", "Resonators",
                "Exhaust Tips", "Exhaust Gaskets", "O2 Sensors", "DPF Filters", "Headers", "Heat Shields",
                "Exhaust Hangers", "Performance Exhausts", "Silencers", "EGR Valves"
            ]),
            ("Body Parts & Exterior",
            [
                "Bumpers", "Fenders", "Hoods", "Doors", "Mirrors", "Grilles", "Spoilers", "Windshields",
                "Windows", "Window Regulators", "Body Panels", "Trim", "Moldings", "Emblems", "Paint",
                "Car Covers", "Wiper Blades", "Wiper Motors", "Roof Racks", "Mud Flaps"
            ])
        };

        foreach ((string name, List<string> subcategories) in parentCategories)
        {
            var parent = Category.Create(name);
            allCategories.Add(parent);

#pragma warning disable CA5394
            int subCount = random.Next(Math.Min(8, subcategories.Count), Math.Min(subcategories.Count, 15));
#pragma warning restore CA5394
            foreach (string subName in subcategories.Take(subCount))
            {
                var subCategory = Category.Create(subName, parent.Id);
                parent.AddSubcategory(subCategory);
                allCategories.Add(subCategory);
            }
        }

        const string sql = @"INSERT INTO categories (id, name, image_path, parent_category_id, slug) 
                         VALUES (@Id, @Name, @ImagePath, @ParentCategoryId, @Slug);";

        connection.Execute(sql, allCategories.Select(c => new
        {
            c.Id,
            c.Name,
            c.ImagePath,
            c.ParentCategoryId,
            Slug = c.Slug.Value
        }));
    }

    private static void SeedProducts(IDbConnection connection)
    {
        int productsCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM products");
        if (productsCount > 0)
        {
            return;
        }

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

        var random = new Random();
        var faker = new Faker();
        var products = new List<Product>();
        var productCarModels = new List<(Guid ProductId, Guid CarModelId)>();

        for (int i = 0; i < 500; i++)
        {
            Category category = faker.PickRandom(categories);

            List<CarModel> productModels = [];
#pragma warning disable CA5394
            int modelCount = random.Next(1, 4);
#pragma warning restore CA5394

            for (int j = 0; j < modelCount; j++)
            {
                CarModel model = faker.PickRandom(carModels);
                if (!productModels.Any(m => m.Id == model.Id))
                {
                    productModels.Add(model);
                }
            }

            var product = Product.Create(
                faker.Commerce.ProductName(),
                null,
                faker.Commerce.Ean13(),
                category.Id,
                productModels,
                faker.Random.Decimal(100, 1000),
                faker.Random.Decimal(0, 30),
                faker.Random.Int(0, 100)
            );

            var specFaker = new Faker();
            int specsCount = faker.Random.Int(4, 10);
            var uniqueSpecNames = new HashSet<string>();

            for (int j = 0; j < specsCount; j++)
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

            products.Add(product);

            foreach (CarModel model in productModels)
            {
                productCarModels.Add((product.Id, model.Id));
            }
        }

        const string insertProductsSql = """
        INSERT INTO products 
        (id, category_id, name, vendor_code, price, discount, stock, is_available, slug)
        VALUES (@Id, @CategoryId, @Name, @VendorCode, @Price, @Discount, @Stock, @IsAvailable, @Slug);
        """;

        const string insertProductCarModelsSql = """
        INSERT INTO product_car_models
        (product_id, car_model_id)
        VALUES (@ProductId, @CarModelId);
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
            p.Name,
            p.VendorCode,
            p.Price,
            p.Discount,
            p.Stock,
            p.IsAvailable,
            Slug = p.Slug.Value
        }));

        connection.Execute(insertProductCarModelsSql, productCarModels.Select(pcm => new
        {
            pcm.ProductId,
            pcm.CarModelId
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
