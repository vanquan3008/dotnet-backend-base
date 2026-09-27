using MyProject.Domain.Entities;
using MyProject.Domain.ValueObjects.Users;

namespace MyProject.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByCodeAsync(Code code, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}
