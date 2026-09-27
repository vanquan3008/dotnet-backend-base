using MyProject.Domain.Common;

namespace MyProject.Domain.Events.Users;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
