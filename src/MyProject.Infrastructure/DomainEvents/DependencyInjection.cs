using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.DomainEvents;

internal static class DomainEventsDependencyInjection
{
    public static IServiceCollection AddDomainEvents(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }
}
