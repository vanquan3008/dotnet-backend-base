using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MyProject.Infrastructure.Identity;

namespace MyProject.Api.Extensions;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(IdentityOptions.SectionName);
        if (!section.GetValue<Boolean>(nameof(IdentityOptions.Enabled)))
        {
            services.AddAuthorization();
            return services;
        }

        var issuer = section[nameof(IdentityOptions.Issuer)] ?? throw new InvalidOperationException("Identity:Issuer is missing.");
        var audience = section[nameof(IdentityOptions.Audience)] ?? throw new InvalidOperationException("Identity:Audience is missing.");
        var signingKey = section[nameof(IdentityOptions.SigningKey)] ?? throw new InvalidOperationException("Identity:SigningKey is missing.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        services
            .AddAuthorizationBuilder()
            .SetFallbackPolicy(
                new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build());

        return services;
    }
}
