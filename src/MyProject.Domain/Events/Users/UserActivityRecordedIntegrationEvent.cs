namespace MyProject.Application.UserActivities.IntegrationEvents;

public sealed record UserActivityRecordedIntegrationEvent(
    Guid ActivityId,
    Guid UserId,
    string Type,
    string? Details,
    DateTimeOffset OccurredAt);
