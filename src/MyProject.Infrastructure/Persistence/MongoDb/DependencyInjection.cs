using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyProject.Application.UserActivities.Interfaces;
using MyProject.Infrastructure.Persistence.MongoDb.UserActivities;

namespace MyProject.Infrastructure.Persistence.MongoDb;

internal static class MongoDbDependencyInjection
{
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(MongoDbOptions.SectionName);
        if (!section.GetValue<bool>(nameof(MongoDbOptions.Enabled)))
        {
            return services;
        }

        services.AddOptions<MongoDbOptions>()
            .Bind(section)
            .Validate(options => Uri.TryCreate(options.ConnectionString, UriKind.Absolute, out _),
                "MongoDb:ConnectionString must be a valid absolute URI.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.DatabaseName),
                "MongoDb:DatabaseName is required.")
            .ValidateOnStart();
        services.AddSingleton<IMongoClient>(serviceProvider =>
            new MongoClient(serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value.ConnectionString));
        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            return serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(options.DatabaseName);
        });
        services.AddScoped<IUserActivityStore, MongoUserActivityStore>();
        services.AddHostedService<UserActivityIndexes>();
        services.AddHealthChecks().AddCheck<MongoHealthCheck>("mongodb");

        return services;
    }
}
