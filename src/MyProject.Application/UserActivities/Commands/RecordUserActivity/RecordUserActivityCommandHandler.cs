using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Messaging.Commands;
using MyProject.Application.UserActivities.Interfaces;
using MyProject.Application.UserActivities.Models;
using MyProject.Application.Users.Interfaces;

namespace MyProject.Application.UserActivities.Commands.RecordUserActivity;

public sealed class RecordUserActivityCommandHandler(
    IUserQueries users,
    IUserActivityStore activities,
    TimeProvider timeProvider)
    : ICommandHandler<RecordUserActivityCommand, UserActivity>
{
    public async Task<UserActivity> Handle(
        RecordUserActivityCommand request,
        CancellationToken cancellationToken)
    {
        if (request.UserId == Guid.Empty)
        {
            throw new RequestValidationException("activity.user_required", "User id is required.");
        }

        var type = request.Type?.Trim();
        if (string.IsNullOrWhiteSpace(type) || type.Length > UserActivity.TypeMaxLength)
        {
            throw new RequestValidationException(
                "activity.type_invalid",
                $"Activity type is required and must not exceed {UserActivity.TypeMaxLength} characters.");
        }

        var details = string.IsNullOrWhiteSpace(request.Details) ? null : request.Details.Trim();
        if (details?.Length > UserActivity.DetailsMaxLength)
        {
            throw new RequestValidationException(
                "activity.details_too_long",
                $"Activity details must not exceed {UserActivity.DetailsMaxLength} characters.");
        }

        if (request.ActivityId == Guid.Empty)
        {
            throw new RequestValidationException("activity.id_required", "Activity id must not be empty.");
        }

        if (request.OccurredAt == default(DateTimeOffset))
        {
            throw new RequestValidationException(
                "activity.occurred_at_invalid",
                "Activity occurrence time is invalid.");
        }

        if (!await users.ExistsAsync(request.UserId, cancellationToken))
        {
            throw new NotFoundException("user.not_found", "User was not found.");
        }

        var activity = new UserActivity(
            request.ActivityId ?? Guid.NewGuid(),
            request.UserId,
            type,
            details,
            (request.OccurredAt ?? timeProvider.GetUtcNow()).ToUniversalTime());
        await activities.AddAsync(activity, cancellationToken);
        return activity;
    }
}
