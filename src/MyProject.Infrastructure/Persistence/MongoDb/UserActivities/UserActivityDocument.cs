using MongoDB.Bson.Serialization.Attributes;

namespace MyProject.Infrastructure.Persistence.MongoDb.UserActivities;

internal sealed class UserActivityDocument
{
    public const string CollectionName = "user_activities";

    [BsonId]
    public Guid Id { get; init; }

    [BsonElement("userId")]
    public Guid UserId { get; init; }

    [BsonElement("type")]
    public string Type { get; init; } = string.Empty;

    [BsonElement("details")]
    public string? Details { get; init; }

    [BsonElement("occurredAtUtc")]
    public DateTime OccurredAtUtc { get; init; }
}
