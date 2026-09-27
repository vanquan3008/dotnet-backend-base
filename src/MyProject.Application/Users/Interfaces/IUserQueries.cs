using MyProject.Application.Users.Models;

namespace MyProject.Application.Users.Interfaces;

public interface IUserQueries
{
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}
