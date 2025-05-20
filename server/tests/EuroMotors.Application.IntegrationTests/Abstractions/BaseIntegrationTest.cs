using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace EuroMotors.Application.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest
{
    private readonly IServiceScope _scope;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IntegrationTestWebAppFactory Factory;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Factory = factory;
        ServiceProvider = _scope.ServiceProvider;
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected async Task CleanDatabaseAsync()
    {
        var tableNames = new List<string>
        {
            "orders",
            "carts", 
            "users",
            "products",
            "categories",
            "car_models",
            "car_brands"
        };
        
        foreach (string tableName in tableNames)
        {
            try
            {
                #pragma warning disable EF1002 
                await DbContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{tableName}\";");
                #pragma warning restore EF1002
            }
            catch
            {
            }
        }
    }
}
