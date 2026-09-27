using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Caching;

internal static class CachingDependencyInjection
{
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(RedisOptions.SectionName);
        if (!section.GetValue<bool>(nameof(RedisOptions.Enabled)))
        {
            return services;
        }

        services.AddOptions<RedisOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Redis:ConnectionString is required.")
            .Validate(options => options.DefaultExpirationMinutes > 0,
                "Redis:DefaultExpirationMinutes must be greater than zero.")
            .ValidateOnStart();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = section[nameof(RedisOptions.ConnectionString)];
            options.InstanceName = section[nameof(RedisOptions.InstanceName)];
        });
        services.AddSingleton<ICacheService, RedisCacheService>();
        return services;
    }
}
