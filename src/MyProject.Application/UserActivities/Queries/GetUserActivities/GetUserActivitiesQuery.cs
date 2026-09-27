using System.ComponentModel.DataAnnotations;
using MyProject.Application.Common.Messaging.Queries;
using MyProject.Application.UserActivities.Models;

namespace MyProject.Application.UserActivities.Queries.GetUserActivities;

public sealed record GetUserActivitiesQuery: IQuery<IReadOnlyList<UserActivity>>
{
    public Guid UserId { get; init; }

    [Range(1, 100)]
    public int Limit { get; init; } = 50;

    public GetUserActivitiesQuery(Guid userId, int limit = 50)
    {
        UserId = userId;
        Limit = limit;
    }
}
