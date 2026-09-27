using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Infrastructure.BackgroundJobs;
using MyProject.Infrastructure.Caching;
using MyProject.Infrastructure.DomainEvents;
using MyProject.Infrastructure.Email;
using MyProject.Infrastructure.ExternalServices;
using MyProject.Infrastructure.FileStorage;
using MyProject.Infrastructure.Identity;
using MyProject.Infrastructure.Messaging.RabbitMq;
using MyProject.Infrastructure.Observability.OpenTelemetry;
using MyProject.Infrastructure.Persistence.MongoDb;
using MyProject.Infrastructure.Persistence.PostgreSql;
using MyProject.Infrastructure.Time;

namespace MyProject.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName = "MyProject")
    {
        services.AddSystemTime();
        services.AddDomainEvents();
        services.AddPostgreSql(configuration);
        services.AddMongoDb(configuration);
        services.AddCaching(configuration);
        services.AddRabbitMq(configuration);
        services.AddEmail(configuration);
        services.AddFileStorage(configuration);
        services.AddIdentityServices(configuration);
        services.AddBackgroundJobs(configuration);
        services.AddExternalServices();
        services.AddObservability(configuration, serviceName);

        return services;
    }
}
