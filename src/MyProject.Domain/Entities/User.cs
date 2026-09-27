using MyProject.Domain.Common;
using MyProject.Domain.Enums;
using MyProject.Domain.Errors.Users;
using MyProject.Domain.Events.Users;
using MyProject.Domain.Exceptions;
using MyProject.Domain.ValueObjects.Users;

namespace MyProject.Domain.Entities;

public sealed class User : Entity<Guid>, IAuditableEntity
{
    public Code Code { get; private set; } = null!;
    public Name Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ModifiedAt { get; private set; }

    private User() { }

    private User(Guid id, Code code, Name name, Email email)
    {
        Id = id;
        Code = code;
        Name = name;
        Email = email;
        Status = UserStatus.Active;
    }

    public static User Create(Guid id, Code code, Name name, Email email)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException(UserErrorCodes.IdRequired, "User id is required.");
        }

        var user = new User(id, code, name, email);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(id));
        return user;
    }

    public void Activate() => ChangeStatus(UserStatus.Active);
    public void Deactivate() => ChangeStatus(UserStatus.Inactive);
    public void Suspend() => ChangeStatus(UserStatus.Suspended);
    public void Delete() => ChangeStatus(UserStatus.Deleted);

    public void RecordLogin(DateTimeOffset loginAt)
    {
        if (Status != UserStatus.Active)
        {
            throw new DomainException(
                UserErrorCodes.NotActive,
                "Only active users can login.",
                DomainErrorType.Conflict);
        }

        LastLoginAt = loginAt.ToUniversalTime();
    }

    private void ChangeStatus(UserStatus status)
    {
        if (Status == UserStatus.Deleted)
        {
            throw new DomainException(
                UserErrorCodes.Deleted,
                "Deleted user cannot be modified.",
                DomainErrorType.Conflict);
        }

        Status = status;
    }

    void IAuditableEntity.MarkCreated(DateTimeOffset at) => CreatedAt = at;
    void IAuditableEntity.MarkModified(DateTimeOffset at) => ModifiedAt = at;
}
