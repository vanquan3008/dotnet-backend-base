using MyProject.Application.UserActivities.Models;

namespace MyProject.Api.Features.UserActivities.Responses;

public sealed record UserActivityResponse(
    Guid Id,
    Guid UserId,
    string Type,
    string? Details,
    DateTimeOffset OccurredAt)
{
    public static UserActivityResponse From(UserActivity activity)
        => new UserActivityResponse(
            activity.Id,
            activity.UserId,
            activity.Type,
            activity.Details,
            activity.OccurredAt);
}
