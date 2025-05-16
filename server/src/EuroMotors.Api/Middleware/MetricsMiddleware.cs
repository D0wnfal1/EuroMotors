namespace EuroMotors.Api.Middleware;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;

    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/prometheus-metrics"))
        {
            context.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";

            await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(context.Response.Body);
            return;
        }

        await _next(context);
    }
}

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UsePrometheusMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}
