using System.Data;
using Bogus;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Carts;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
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
        SeedCarts(connection);
        SeedCartItems(connection);
        SeedOrders(connection);
        SeedPayments(connection);
    }

    private static void SeedCarModels(IDbConnection connection)
    {
        Faker<CarModel>? faker = new Faker<CarModel>()
            .CustomInstantiator(f => CarModel.Create(
                f.Vehicle.Manufacturer(),
                f.Vehicle.Model()
                ));

        List<CarModel>? carModels = faker.Generate(10);

        const string sql = @"INSERT INTO car_models (id, brand, model) VALUES (@Id, @Brand, @Model);";

        connection.Execute(sql, carModels);
    }

    private static void SeedCategories(IDbConnection connection)
    {
        Faker<Category>? faker = new Faker<Category>()
            .CustomInstantiator(f => Category.Create(
                f.Commerce.Categories(1)[0]
                ));

        List<Category>? categories = faker.Generate(10);

        const string sql = @"INSERT INTO categories (id, name, is_archived) VALUES (@Id, @Name, @IsArchived);";

        connection.Execute(sql, categories);
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
    private static void SeedCarts(IDbConnection connection)
    {
        var users = connection.Query<User>("SELECT id FROM users").ToList();
        var products = connection.Query<Product>("SELECT id, price FROM products").ToList();

        if (users.Count == 0 || products.Count == 0)
        {
            return;
        }

        Faker<Cart> cartFaker = new Faker<Cart>()
            .CustomInstantiator(f =>
            {
                User user = users[f.Random.Int(0, users.Count - 1)];
                return Cart.Create(user.Id);
            });

        List<Cart> carts = cartFaker.Generate(10);

        const string cartSql = @"INSERT INTO carts (id, user_id) VALUES (@Id, @UserId);";

        connection.Execute(cartSql, carts);
    }

    private static void SeedCartItems(IDbConnection connection)
    {
        var carts = connection.Query<Cart>("SELECT id FROM carts").ToList();
        var products = connection.Query<Product>("SELECT id, price FROM products").ToList();

        if (carts.Count == 0 || products.Count == 0)
        {
            return;
        }

        Faker<CartItem> cartItemFaker = new Faker<CartItem>()
            .CustomInstantiator(f =>
            {
                Cart cart = carts[f.Random.Int(0, carts.Count - 1)];
                Product product = products[f.Random.Int(0, products.Count - 1)];
                int quantity = f.Random.Int(1, 3);

                return CartItem.Create(product, cart.Id, quantity);
            });

        List<CartItem> cartItems = cartItemFaker.Generate(30);

        const string cartItemSql = """
                                   INSERT INTO cart_items (id, product_id, cart_id, quantity, unit_price)
                                   VALUES (@Id, @ProductId, @CartId, @Quantity, @UnitPrice);
                                   """;

        var cartItemData = cartItems.Select(ci => new
        {
            ci.Id,
            ci.ProductId,
            ci.CartId,
            ci.Quantity,
            ci.UnitPrice,
        }).ToList();

        connection.Execute(cartItemSql, cartItemData);
    }

    private static void SeedOrders(IDbConnection connection)
    {
        var users = connection.Query<User>("SELECT id FROM users").ToList();
        var products = connection.Query<Product>("SELECT id, price FROM products").ToList();

        if (users.Count == 0 || products.Count == 0)
        {
            return;
        }

        Faker<Order> orderFaker = new Faker<Order>()
            .CustomInstantiator(f =>
            {
                User user = users[f.Random.Int(0, users.Count - 1)];
                return Order.Create(user.Id);
            });

        List<Order> orders = orderFaker.Generate(10);

        const string orderSql = @"INSERT INTO orders (id, user_id, status, total_price, products_issued, created_at_utc, updated_at_utc) VALUES (@Id, @UserId, @Status, @TotalPrice, @ProductsIssued, @CreatedAtUtc, @UpdatedAtUtc);";
        connection.Execute(orderSql, orders.Select(o => new
        {
            o.Id,
            o.UserId,
            o.Status,
            o.TotalPrice,
            ProductsIssued = false, 
            o.CreatedAtUtc,
            o.UpdatedAtUtc
        }));

        Faker<OrderItem> orderItemFaker = new Faker<OrderItem>()
            .CustomInstantiator(f =>
            {
                Order order = orders[f.Random.Int(0, orders.Count - 1)];
                Product product = products[f.Random.Int(0, products.Count - 1)];
                decimal quantity = f.Random.Decimal(1, 3);
                decimal unitPrice = product.Price;

                return OrderItem.Create(order.Id, product.Id, quantity, unitPrice);
            });

        List<OrderItem> orderItems = orderItemFaker.Generate(30);

        const string orderItemSql = """
                                    INSERT INTO order_items (id, order_id, product_id, quantity, unit_price, price)
                                    VALUES (@Id, @OrderId, @ProductId, @Quantity, @UnitPrice, @Price);
                                """;

        var orderItemData = orderItems.Select(oi => new
        {
            oi.Id,
            oi.OrderId,
            oi.ProductId,
            oi.Quantity,
            oi.UnitPrice,
            oi.Price
        }).ToList();

        connection.Execute(orderItemSql, orderItemData);
    }

    private static void SeedPayments(IDbConnection connection)
    {
        var orders = connection.Query<Order>("SELECT id, total_price FROM orders").ToList();

        if (orders.Count == 0)
        {
            return;
        }

        Faker<Payment> paymentFaker = new Faker<Payment>()
            .CustomInstantiator(f =>
            {
                Order order = orders[f.Random.Int(0, orders.Count - 1)];
                var transactionId = Guid.NewGuid();

                decimal amount = Math.Round(order.TotalPrice * f.Random.Decimal(0.5m, 1m), 2);

                DateTime createdAt = f.Date.Past(1, DateTime.UtcNow).ToUniversalTime();

                var payment = Payment.Create(order, transactionId, amount);
                typeof(Payment).GetProperty(nameof(Payment.CreatedAtUtc))?.SetValue(payment, createdAt);

                return payment;
            });

        List<Payment> payments = paymentFaker.Generate(10);

        var uniquePayments = payments.GroupBy(p => p.OrderId).Select(g => g.First()).ToList();

        const string paymentSql = """
                                  INSERT INTO payments (id, order_id, transaction_id, amount, amount_refunded, created_at_utc, refunded_at_utc)
                                  VALUES (@Id, @OrderId, @TransactionId, @Amount, @AmountRefunded, @CreatedAtUtc, @RefundedAtUtc);
                                  """;

        var paymentData = uniquePayments.Select(p => new
        {
            p.Id,
            p.OrderId,
            p.TransactionId,
            p.Amount,
            p.AmountRefunded,
            p.CreatedAtUtc,
            p.RefundedAtUtc
        });

        connection.Execute(paymentSql, paymentData);
    }
}
