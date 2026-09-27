using System.Diagnostics;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using MyProject.Api.Configuration;
using MyProject.Api.ExceptionHandling;

namespace MyProject.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    internal const string CorsPolicy = "api-cors";

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request validation failed.",
                    Instance = context.HttpContext.Request.Path
                };
                problemDetails.Extensions["code"] = "request.invalid";
                problemDetails.Extensions["traceId"] =
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                return new BadRequestObjectResult(problemDetails);
            });
        services.AddOpenApi();

        services.AddHealthChecks();
        AddCors(services, configuration);
        AddRateLimiting(services, configuration);
        AddRequestTimeout(services, configuration);
        return services;
    }

    

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(CorsOptions.SectionName);
        if (!section.GetValue<Boolean>(nameof(CorsOptions.Enabled)))
        {
            return;
        }

        var origins = section.GetSection(nameof(CorsOptions.AllowedOrigins)).Get<string[]>() ?? [];
        if (origins.Length == 0)
        {
            throw new InvalidOperationException("Api:Cors:AllowedOrigins is required when CORS is enabled.");
        }

        services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));
    }

    private static void AddRateLimiting(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(RateLimitingOptions.SectionName);
        if (!section.GetValue(nameof(RateLimitingOptions.Enabled), true))
        {
            return;
        }

        var permitLimit = section.GetValue(nameof(RateLimitingOptions.PermitLimit), 100);
        var windowSeconds = section.GetValue(nameof(RateLimitingOptions.WindowSeconds), 60);
        if (permitLimit <= 0 || windowSeconds <= 0)
        {
            throw new InvalidOperationException("Api:RateLimiting values must be greater than zero.");
        }

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? context.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });
    }

    private static void AddRequestTimeout(IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(ApiRequestTimeoutOptions.SectionName);
        if (!section.GetValue(nameof(ApiRequestTimeoutOptions.Enabled), true))
        {
            return;
        }

        var seconds = section.GetValue(nameof(ApiRequestTimeoutOptions.Seconds), 30);
        if (seconds <= 0)
        {
            throw new InvalidOperationException("Api:RequestTimeout:Seconds must be greater than zero.");
        }

        services.AddRequestTimeouts(options => options.DefaultPolicy = new RequestTimeoutPolicy
        {
            Timeout = TimeSpan.FromSeconds(seconds),
            TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
        });
    }
}
