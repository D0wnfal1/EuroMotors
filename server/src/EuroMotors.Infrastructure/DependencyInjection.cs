using System.Text;
using Dapper;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Callback;
using EuroMotors.Application.Abstractions.Clock;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Delivery;
using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Application.Carts;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;
using EuroMotors.Infrastructure.Authentication;
using EuroMotors.Infrastructure.Authorization;
using EuroMotors.Infrastructure.Caching;
using EuroMotors.Infrastructure.Callback;
using EuroMotors.Infrastructure.Database;
using EuroMotors.Infrastructure.Delivery;
using EuroMotors.Infrastructure.Payments;
using EuroMotors.Infrastructure.Repositories;
using EuroMotors.Infrastructure.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace EuroMotors.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddCaching(configuration)
            .AddHealthChecks(configuration)
            .AddPayment(configuration)
            .AddDelivery(configuration)
            .AddCallback(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IProductImageRepository, ProductImageRepository>();

        services.AddScoped<ICarModelRepository, CarModelRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<CartService>();

        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database") ??
                                   throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton<IDbConnectionFactory>(_ =>
            new DbConnectionFactory(connectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Cache") ??
                                   throw new ArgumentNullException(nameof(configuration));
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Cache")!);

        return services;
    }

    private static IServiceCollection AddPayment(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PaymentOptions>(configuration.GetSection("Payment"));

        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }

    private static IServiceCollection AddDelivery(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<DeliveryOptions>(configuration.GetSection("Delivery"));

        services.AddHttpClient<IDeliveryService, DeliveryService>();

        services.AddScoped<IDeliveryService, DeliveryService>();

        return services;
    }

    private static IServiceCollection AddCallback(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CallbackOptions>(configuration.GetSection("Callback"));

        services.AddHttpClient<ICallbackService, CallbackService>();

        services.AddScoped<ICallbackService, CallbackService>();

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string? token = context.Request.Cookies["AccessToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        return services;
    }
}
