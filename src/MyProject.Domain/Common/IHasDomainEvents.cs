namespace MyProject.Domain.Common;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
}
