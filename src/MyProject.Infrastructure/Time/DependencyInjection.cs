using Microsoft.Extensions.DependencyInjection;

namespace MyProject.Infrastructure.Time;

internal static class TimeDependencyInjection
{
    public static IServiceCollection AddSystemTime(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
