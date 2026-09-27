using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Identity;

internal static class IdentityDependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(IdentityOptions.SectionName);
        if (!section.GetValue<bool>(nameof(IdentityOptions.Enabled)))
        {
            return services;
        }

        services.AddOptions<IdentityOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Identity:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Identity:Audience is required.")
            .Validate(options => options.SigningKey.Length >= 32,
                "Identity:SigningKey must contain at least 32 characters.")
            .Validate(options => options.AccessTokenMinutes > 0,
                "Identity:AccessTokenMinutes must be greater than zero.")
            .ValidateOnStart();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        return services;
    }
}
