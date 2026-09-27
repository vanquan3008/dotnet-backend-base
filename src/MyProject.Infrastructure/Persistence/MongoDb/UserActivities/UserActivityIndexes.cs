using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace MyProject.Infrastructure.Persistence.MongoDb.UserActivities;

internal sealed class UserActivityIndexes(IMongoDatabase database) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<UserActivityDocument>(UserActivityDocument.CollectionName);
        var keys = Builders<UserActivityDocument>.IndexKeys
            .Ascending(activity => activity.UserId)
            .Descending(activity => activity.OccurredAtUtc);
        var index = new CreateIndexModel<UserActivityDocument>(
            keys,
            new CreateIndexOptions { Name = "ix_user_activities_user_occurred_at" });

        await collection.Indexes.CreateOneAsync(index, cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
