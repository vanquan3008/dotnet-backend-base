namespace MyProject.Contracts.UserActivities;

public sealed record UserActivityRecordedIntegrationEvent(
    Guid ActivityId,
    Guid UserId,
    string Type,
    string? Details,
    DateTimeOffset OccurredAt);
