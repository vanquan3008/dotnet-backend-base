using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Results;
using MyProject.Domain.Exceptions;

namespace MyProject.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = exception switch
        {
            DomainException domainException => domainException.ToError(),
            ApplicationExceptionBase applicationException => applicationException.ToError(),
            _ => Error.Failure("server.unexpected", "An unexpected error occurred.")
        };

        if (exception is not (DomainException or ApplicationExceptionBase))
        {
            logger.LogError(
                exception,
                "Unhandled exception while processing {RequestMethod} {RequestPath}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        var statusCode = error.Type.ToStatusCode();
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode >= 500 ? "An unexpected error occurred." : "Request failed.",
            Detail = error.Message,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
