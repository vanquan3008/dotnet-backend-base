using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Infrastructure.Observability.Logging;
using MyProject.Infrastructure.Observability.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MyProject.Infrastructure.Observability.OpenTelemetry;

internal static class OpenTelemetryDependencyInjection
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddObservabilityLogging();
        var openTelemetry = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics.AddApplicationMetrics());

        if (configuration.GetValue<bool>("OpenTelemetry:Otlp:Enabled"))
        {
            openTelemetry.UseOtlpExporter();
        }
        return services;
    }
}
