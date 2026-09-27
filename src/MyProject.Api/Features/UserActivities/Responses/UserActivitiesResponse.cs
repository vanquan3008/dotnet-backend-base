using MyProject.Application.UserActivities.Models;

namespace MyProject.Api.Features.UserActivities.Responses;

public sealed record UserActivitiesResponse(IReadOnlyList<UserActivityResponse> Items)
{
    public static UserActivitiesResponse From(IEnumerable<UserActivity> activities)
        => new(activities.Select(UserActivityResponse.From).ToArray());
}
