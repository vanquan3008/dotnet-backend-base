using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.BackgroundJobs;

internal static class BackgroundJobsDependencyInjection
{
    public static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(BackgroundJobOptions.SectionName);
        if (!section.GetValue<bool>(nameof(BackgroundJobOptions.Enabled)))
        {
            return services;
        }

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is required when BackgroundJobs is enabled.");
        var queue = section[nameof(BackgroundJobOptions.Queue)] ?? "default";
        services.AddOptions<BackgroundJobOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Queue),
                "BackgroundJobs:Queue is required.")
            .ValidateOnStart();
        services.AddHangfire(configurator => configurator
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));
        services.AddHangfireServer(options => options.Queues = [queue]);
        services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
        return services;
    }
}
