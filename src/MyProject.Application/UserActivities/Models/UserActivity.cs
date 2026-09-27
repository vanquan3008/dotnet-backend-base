namespace MyProject.Application.UserActivities.Models;

public sealed record UserActivity(
    Guid Id,
    Guid UserId,
    string Type,
    string? Details,
    DateTimeOffset OccurredAt)
{
    public const int TypeMaxLength = 100;
    public const int DetailsMaxLength = 2000;
}
