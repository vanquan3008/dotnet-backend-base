using MyProject.Application.UserActivities.Models;

namespace MyProject.Application.UserActivities.Interfaces;

public interface IUserActivityStore
{
    Task AddAsync(UserActivity activity, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserActivity>> GetRecentAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken);
}
