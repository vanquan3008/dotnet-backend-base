using MyProject.Application.Common.Messaging.Commands;
using MyProject.Application.UserActivities.Models;

namespace MyProject.Application.UserActivities.Commands.RecordUserActivity;

public sealed record RecordUserActivityCommand(
    Guid UserId,
    string Type,
    string? Details,
    Guid? ActivityId = null,
    DateTimeOffset? OccurredAt = null)
    : ICommand<UserActivity>;
