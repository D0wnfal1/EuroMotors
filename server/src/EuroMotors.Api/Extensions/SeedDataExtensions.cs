using System.Data;
using Bogus;
using Dapper;
using EuroMotors.Application.Abstractions.Data;

namespace EuroMotors.Api.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IDbConnectionFactory sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();

        // var faker = new Faker();

        const string sql = ""; 

        connection.Execute(sql);
    }
}
