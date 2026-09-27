using Microsoft.EntityFrameworkCore;
using MyProject.Domain.Entities;
using MyProject.Domain.Repositories;
using MyProject.Domain.ValueObjects.Users;
using UserEmail = MyProject.Domain.ValueObjects.Users.Email;

namespace MyProject.Infrastructure.Persistence.PostgreSql.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(Code code, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(user => user.Code == code, cancellationToken);
    }
    public Task<bool> ExistsByEmailAsync(UserEmail email, CancellationToken cancellationToken)
    {
        return context.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        return context.Users.AddAsync(user, cancellationToken).AsTask();
    }
}
