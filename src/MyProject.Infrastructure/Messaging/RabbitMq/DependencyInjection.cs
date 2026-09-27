using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;
using MyProject.Infrastructure.Messaging.Consumers;
using MyProject.Infrastructure.Messaging.Publishers;

namespace MyProject.Infrastructure.Messaging.RabbitMq;

internal static class RabbitMqDependencyInjection
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(MessagingOptions.SectionName);
        if (!section.GetValue<bool>(nameof(MessagingOptions.Enabled)))
        {
            return services;
        }

        if (!configuration.GetValue("PostgreSql:Enabled", true)
            || !configuration.GetValue<bool>("MongoDb:Enabled"))
        {
            throw new InvalidOperationException(
                "Messaging consumers require PostgreSql and MongoDb to be enabled.");
        }

        var options = section.Get<MessagingOptions>() ?? new MessagingOptions();
        services.AddOptions<MessagingOptions>()
            .Bind(section)
            .Validate(value => !string.IsNullOrWhiteSpace(value.Host), "Messaging:Host is required.")
            .Validate(value => !string.IsNullOrWhiteSpace(value.Username), "Messaging:Username is required.")
            .Validate(value => !string.IsNullOrWhiteSpace(value.Password), "Messaging:Password is required.")
            .Validate(value => value.RetryCount > 0, "Messaging:RetryCount must be greater than zero.")
            .Validate(value => value.RetryIntervalSeconds > 0,
                "Messaging:RetryIntervalSeconds must be greater than zero.")
            .Validate(value => value.ConcurrentMessageLimit > 0,
                "Messaging:ConcurrentMessageLimit must be greater than zero.")
            .ValidateOnStart();
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.AddConsumer<UserActivityRecordedConsumer>(consumer =>
                consumer.ConcurrentMessageLimit = options.ConcurrentMessageLimit);
            configurator.UsingRabbitMq((context, rabbitMq) =>
            {
                rabbitMq.Host(options.Host, options.VirtualHost, host =>
                {
                    host.Username(options.Username);
                    host.Password(options.Password);
                });
                rabbitMq.UseMessageRetry(retry => retry.Interval(
                    options.RetryCount,
                    TimeSpan.FromSeconds(options.RetryIntervalSeconds)));
                rabbitMq.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IMessagePublisher, MassTransitPublisher>();
        return services;
    }
}
