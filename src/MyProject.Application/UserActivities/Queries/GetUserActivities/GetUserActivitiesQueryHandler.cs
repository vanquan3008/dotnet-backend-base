using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Messaging.Queries;
using MyProject.Application.UserActivities.Interfaces;
using MyProject.Application.UserActivities.Models;

namespace MyProject.Application.UserActivities.Queries.GetUserActivities;

public sealed class GetUserActivitiesQueryHandler(IUserActivityStore activities) : IQueryHandler<GetUserActivitiesQuery, IReadOnlyList<UserActivity>>
{
    public Task<IReadOnlyList<UserActivity>> Handle(
        GetUserActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty || request.Limit is < 1 or > 100)
        {
            throw new RequestValidationException(
                "activity.query_invalid",
                "User id is required and limit must be between 1 and 100.");
        }
        return activities.GetRecentAsync(request.UserId, request.Limit, cancellationToken);
    }
}
