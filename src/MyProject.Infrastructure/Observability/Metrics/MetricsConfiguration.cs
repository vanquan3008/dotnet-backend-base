using OpenTelemetry.Metrics;

namespace MyProject.Infrastructure.Observability.Metrics;

internal static class MetricsConfiguration
{
    public static MeterProviderBuilder AddApplicationMetrics(this MeterProviderBuilder metrics)
        => metrics.AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation();
}
