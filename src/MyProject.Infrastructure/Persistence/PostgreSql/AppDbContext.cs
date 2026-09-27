using Microsoft.EntityFrameworkCore;
using MyProject.Application.Common.Exceptions;
using MyProject.Application.Common.Interfaces;
using MyProject.Domain.Common;
using MyProject.Domain.Entities;
using MyProject.Domain.Errors.Users;
using Npgsql;

namespace MyProject.Infrastructure.Persistence.PostgreSql;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    TimeProvider timeProvider,
    IDomainEventDispatcher? domainEventDispatcher = null) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    public override int SaveChanges()
        => throw new NotSupportedException("Use SaveChangesAsync so audit fields are set.");

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
        => throw new NotSupportedException("Use SaveChangesAsync so audit fields are set.");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(true, cancellationToken);

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.MarkCreated(now);
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.MarkModified(now);
            }
        }

        try
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            if (Database.CurrentTransaction is null)
            {
                await DispatchDomainEventsAsync(cancellationToken);
            }
            return result;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres)
        {
            throw postgres.ConstraintName switch
            {
                "ux_users_code" => new ConflictException(
                    UserErrorCodes.CodeAlreadyExists,
                    "User code already exists.",
                    exception),
                "ux_users_email" => new ConflictException(
                    UserErrorCodes.EmailAlreadyExists,
                    "User email already exists.",
                    exception),
                _ => new ConflictException(
                    "database.unique_constraint",
                    "A unique value already exists.",
                    exception)
            };
        }
    }

    internal IReadOnlyCollection<IDomainEvent> DequeueDomainEvents()
        => ChangeTracker.Entries()
            .Select(entry => entry.Entity)
            .OfType<IHasDomainEvents>()
            .SelectMany(entity => entity.DequeueDomainEvents())
            .ToArray();

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = DequeueDomainEvents();
        if (domainEventDispatcher is not null && domainEvents.Count > 0)
        {
            await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }
    }
}
