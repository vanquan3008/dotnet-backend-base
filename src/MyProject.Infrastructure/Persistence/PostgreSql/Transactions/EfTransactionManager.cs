using Microsoft.EntityFrameworkCore;
using MyProject.Application.Common.Interfaces;

namespace MyProject.Infrastructure.Persistence.PostgreSql.Transactions;

internal sealed class EfTransactionManager(
    AppDbContext context,
    IDomainEventDispatcher domainEventDispatcher) : ITransactionManager
{
    public async Task<TResponse> ExecuteAsync<TResponse>(
        Func<CancellationToken, Task<TResponse>> operation,
        CancellationToken cancellationToken = default)
    {
        if (context.Database.CurrentTransaction is not null)
        {
            return await operation(cancellationToken);
        }

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            var domainEvents = context.DequeueDomainEvents();
            if (domainEvents.Count > 0)
            {
                await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
            }
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
