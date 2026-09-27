using System.Runtime.CompilerServices;
using MyProject.Domain.Common;

namespace MyProject.Domain.Entities;

public abstract class Entity<TId> : IEquatable<Entity<TId>>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> domainEvents = [];

    public TId Id { get; protected set; } = default!;

    public bool Equals(Entity<TId>? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (other is null || GetType() != other.GetType() || IsTransient || other.IsTransient)
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);
    public override int GetHashCode()
        => IsTransient ? RuntimeHelpers.GetHashCode(this) : HashCode.Combine(GetType(), Id);
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
        => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
        => !Equals(left, right);

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => domainEvents.Add(domainEvent);

    IReadOnlyCollection<IDomainEvent> IHasDomainEvents.DequeueDomainEvents()
    {
        var events = domainEvents.ToArray();
        domainEvents.Clear();
        return events;
    }

    private bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default!);
}
