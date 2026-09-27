using Microsoft.EntityFrameworkCore;
using MyProject.Application.Users.Interfaces;
using MyProject.Application.Users.Models;

namespace MyProject.Infrastructure.Persistence.PostgreSql.Queries;

internal sealed class UserQueries(AppDbContext context) : IUserQueries
{
    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await context.Users.AsNoTracking()
            .Where(candidate => candidate.Id == id)
            .Select(candidate => new
            {
                candidate.Id,
                candidate.Code,
                candidate.Name,
                candidate.Email,
                candidate.Status,
                candidate.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? null
            : new UserDto(
                user.Id,
                user.Code.Value,
                user.Name.Value,
                user.Email.Value,
                user.Status.ToString(),
                user.CreatedAt);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        => context.Users.AsNoTracking().AnyAsync(user => user.Id == id, cancellationToken);
}
