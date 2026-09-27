using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyProject.Api.Configuration;
using MyProject.Infrastructure.Identity;
using Serilog;
using Serilog.Events;

namespace MyProject.Api.Extensions;

internal static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        var configuration = app.Configuration;

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.000} ms";
            options.GetLevel = (context, _, exception) =>
            {
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    return LogEventLevel.Verbose;
                }

                if (exception is not null || context.Response.StatusCode >= 500)
                {
                    return LogEventLevel.Error;
                }

                return context.Response.StatusCode >= 400
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
            };
            options.EnrichDiagnosticContext = (diagnosticContext, context) =>
            {
                diagnosticContext.Set(
                    "TraceId",
                    Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier);
                diagnosticContext.Set("RequestHost", context.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", context.Request.Scheme);

                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    diagnosticContext.Set("UserId", userId);
                }
            };
        });

        app.UseExceptionHandler();

        if (configuration.GetValue<bool>($"{CorsOptions.SectionName}:{nameof(CorsOptions.Enabled)}"))
        {
            app.UseCors(ServiceCollectionExtensions.CorsPolicy);
        }

        if (configuration.GetValue($"{ApiRequestTimeoutOptions.SectionName}:{nameof(ApiRequestTimeoutOptions.Enabled)}", true))
        {
            app.UseRequestTimeouts();
        }

        if (configuration.GetValue<bool>($"{IdentityOptions.SectionName}:{nameof(IdentityOptions.Enabled)}"))
        {
            app.UseAuthentication();
        }

        if (configuration.GetValue($"{RateLimitingOptions.SectionName}:{nameof(RateLimitingOptions.Enabled)}", true))
        {
            app.UseRateLimiter();
        }

        app.UseAuthorization();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        }).AllowAnonymous();
        app.MapHealthChecks("/health/ready").AllowAnonymous();

        if (app.Environment.IsDevelopment() || configuration.GetValue<bool>("OpenApi:Enabled"))
        {
            app.MapOpenApi().AllowAnonymous();
        }

        app.MapControllers();
        return app;
    }
}
