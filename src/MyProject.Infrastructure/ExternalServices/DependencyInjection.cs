using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.ExternalServices;

internal static class ExternalServicesDependencyInjection
{
    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddHttpClient<IExternalApiClient, ExternalApiClient>();
        return services;
    }
}
