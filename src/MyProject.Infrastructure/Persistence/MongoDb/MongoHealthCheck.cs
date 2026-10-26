using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MyProject.Infrastructure.Persistence.MongoDb;

internal sealed class MongoHealthCheck(IMongoDatabase database) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (MongoException exception)
        {
            return Unavailable(exception);
        }
        catch (TimeoutException exception)
        {
            return Unavailable(exception);
        }
    }

    private static HealthCheckResult Unavailable(Exception exception)
        => HealthCheckResult.Unhealthy("MongoDB is unavailable.", exception);
}
