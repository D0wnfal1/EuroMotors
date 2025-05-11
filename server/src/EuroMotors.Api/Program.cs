using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using EuroMotors.Api;
using EuroMotors.Api.Extensions;
using EuroMotors.Application;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Prometheus;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .ForwardToPrometheus();

if (!builder.Environment.IsEnvironment("Testing"))
{
    X509Certificate2 cert = X509CertificateLoader.LoadPkcs12FromFile(
        "./ssl/euromotors_tech.pfx",
        "656bb9bb-16d7-496c-90a7-5dc7ae559128"
    );

    builder.WebHost.UseKestrel(options =>
    {
        options.ListenAnyIP(80);
        options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps(new HttpsConnectionAdapterOptions
        {
            ClientCertificateMode = ClientCertificateMode.NoCertificate,
            SslProtocols = SslProtocols.Tls12,
            ServerCertificate = cert,
        }));
    });
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    app.UseSwaggerWithUi();
    app.ApplyMigrations();

    IPasswordHasher passwordHasher = app.Services.GetRequiredService<IPasswordHasher>();
    app.SeedData(passwordHasher);
}

app.UseHttpMetrics();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/metrics"))
    {
        context.Response.Headers.Remove("Location");
        await next();
    }
    else
    {
        await next();
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

if (!builder.Environment.IsEnvironment("Testing"))
{
    app.MapHealthChecks("health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    
    app.MapMetrics().WithMetadata(new EuroMotors.Api.HttpsRedirectionDisableMetadata());
    
    app.UseSpaFallback();
}

await app.RunAsync();

namespace EuroMotors.Api
{
    public partial class Program;
    
    public class HttpsRedirectionDisableMetadata
    {
    }
}
