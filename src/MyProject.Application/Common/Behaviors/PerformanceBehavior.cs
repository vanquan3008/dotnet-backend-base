using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MyProject.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long SlowRequestThresholdMilliseconds = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await next(cancellationToken);
        }
        finally
        {
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds >= SlowRequestThresholdMilliseconds)
            {
                logger.LogWarning(
                    "Slow request {RequestName} took {ElapsedMs} ms (threshold: {ThresholdMs} ms)",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds,
                    SlowRequestThresholdMilliseconds);
            }
        }
    }
}
