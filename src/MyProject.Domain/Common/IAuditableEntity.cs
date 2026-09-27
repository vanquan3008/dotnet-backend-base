namespace MyProject.Domain.Common;

public interface IAuditableEntity
{
    void MarkCreated(DateTimeOffset at);
    void MarkModified(DateTimeOffset at);
}
