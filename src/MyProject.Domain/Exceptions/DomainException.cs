namespace MyProject.Domain.Exceptions;

public sealed class DomainException : Exception
{
    public string Code { get; }
    public DomainErrorType Type { get; }

    public DomainException(
        string code,
        string message,
        DomainErrorType type = DomainErrorType.Validation)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "The domain error type is invalid.");
        }

        Code = code;
        Type = type;
    }
}
