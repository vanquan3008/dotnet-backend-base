using MyProject.Application.UserActivities.Models;

namespace MyProject.Infrastructure.Persistence.MongoDb.UserActivities;

internal static class UserActivityMapper
{
    public static UserActivityDocument ToDocument(this UserActivity activity) => new UserActivityDocument()
    {
        Id = activity.Id,
        UserId = activity.UserId,
        Type = activity.Type,
        Details = activity.Details,
        OccurredAtUtc = activity.OccurredAt.UtcDateTime
    };

    public static UserActivity ToApplicationModel(this UserActivityDocument document)
        => new UserActivity(
            document.Id,
            document.UserId,
            document.Type,
            document.Details,
            new DateTimeOffset(document.OccurredAtUtc, TimeSpan.Zero));
}
