using MongoDB.Driver;
using MyProject.Application.UserActivities.Interfaces;
using MyProject.Application.UserActivities.Models;

namespace MyProject.Infrastructure.Persistence.MongoDb.UserActivities;

internal sealed class MongoUserActivityStore(IMongoDatabase database) : IUserActivityStore
{
    private readonly IMongoCollection<UserActivityDocument> activities =
        database.GetCollection<UserActivityDocument>(UserActivityDocument.CollectionName);

    public Task AddAsync(UserActivity activity, CancellationToken cancellationToken)
    {
        var document = activity.ToDocument();
        var update = Builders<UserActivityDocument>.Update
            .SetOnInsert(stored => stored.Id, document.Id)
            .SetOnInsert(stored => stored.UserId, document.UserId)
            .SetOnInsert(stored => stored.Type, document.Type)
            .SetOnInsert(stored => stored.Details, document.Details)
            .SetOnInsert(stored => stored.OccurredAtUtc, document.OccurredAtUtc);
        return activities.UpdateOneAsync(
            stored => stored.Id == document.Id,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);
    }

    public async Task<IReadOnlyList<UserActivity>> GetRecentAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken)
    {
        var documents = await activities.Find(activity => activity.UserId == userId)
            .SortByDescending(activity => activity.OccurredAtUtc)
            .Limit(limit)
            .ToListAsync(cancellationToken);
        return documents.Select(document => document.ToApplicationModel()).ToArray();
    }
}
