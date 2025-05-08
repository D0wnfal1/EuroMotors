using EuroMotors.Api;
using EuroMotors.Api.Extensions;
using EuroMotors.Application;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

//builder.Services.AddCors(options =>
//    options.AddPolicy("Client", corsPolicyBuilder => corsPolicyBuilder
//        .WithOrigins("http://localhost:4200", "https://localhost:4200")
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials()));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();

    app.ApplyMigrations();

    IPasswordHasher passwordHasher = app.Services.GetRequiredService<IPasswordHasher>();
    app.SeedData(passwordHasher);
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

//app.UseCors("Client");

app.UseExceptionHandler();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.UseSpaFallback();

await app.RunAsync();

namespace EuroMotors.Api
{
    public partial class Program;
}
