using Microsoft.Extensions.FileProviders;

namespace EuroMotors.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }

    public static IApplicationBuilder UseSpaFallback(this IApplicationBuilder app)
    {
        app.UseWhen(
            context => !context.Request.Path.StartsWithSegments("/api") &&
                       !context.Request.Path.StartsWithSegments("/health") &&
                       !context.Request.Path.StartsWithSegments("/swagger") &&
                       !Path.HasExtension(context.Request.Path.Value),
            appBuilder => appBuilder.UseSpa(spa => spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            }));

        return app;
    }
}
