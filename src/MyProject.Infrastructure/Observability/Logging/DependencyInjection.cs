using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyProject.Infrastructure.Observability.Logging;

internal static class LoggingDependencyInjection
{
    public static IServiceCollection AddObservabilityLogging(this IServiceCollection services)
    {
        services.Configure<LoggerFactoryOptions>(options =>
            options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId
                | ActivityTrackingOptions.SpanId
                | ActivityTrackingOptions.ParentId);
        return services;
    }
}
