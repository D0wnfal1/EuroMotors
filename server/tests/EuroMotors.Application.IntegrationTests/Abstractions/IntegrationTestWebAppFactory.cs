using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using EuroMotors.Api;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Infrastructure.Database;
using EuroMotors.Infrastructure.DomainEvents;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace EuroMotors.Application.IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("euromotors")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private readonly X509Certificate2 _testCertificate;

    public IntegrationTestWebAppFactory()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("cn=Test Certificate", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        _testCertificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString(),
                ["ConnectionStrings:Cache"] = _redisContainer.GetConnectionString()
            }));

        builder.ConfigureTestServices(services =>
        {
            // Register domain events dispatcher first
            services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();

            // Then remove and register DbContext
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(_dbContainer.GetConnectionString())
                    .UseSnakeCaseNamingConvention());

            services.RemoveAll<IDbConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new DbConnectionFactory(_dbContainer.GetConnectionString()));

            services.Configure<RedisCacheOptions>(redisCacheOptions =>
                redisCacheOptions.Configuration = _redisContainer.GetConnectionString());

            services.AddSingleton(_testCertificate);
        });

        builder.UseEnvironment("Testing");

        builder.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(80);
            options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps(new HttpsConnectionAdapterOptions
            {
                ServerCertificate = _testCertificate
            }));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        _testCertificate.Dispose();
    }
}
