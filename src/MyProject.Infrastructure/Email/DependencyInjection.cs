using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Email;

internal static class EmailDependencyInjection
{
    public static IServiceCollection AddEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(EmailOptions.SectionName);
        if (!section.GetValue<bool>(nameof(EmailOptions.Enabled)))
        {
            return services;
        }

        services.AddOptions<EmailOptions>()
            .Bind(section)
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                "Email:BaseUrl must be a valid absolute URI.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.ApiKey), "Email:ApiKey is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SenderEmail),
                "Email:SenderEmail is required.")
            .ValidateOnStart();
        services.AddHttpClient<IEmailService, BrevoEmailService>(client =>
            client.BaseAddress = new Uri(section[nameof(EmailOptions.BaseUrl)]!));
        return services;
    }
}
