using MyProject.Application.Common.Results;

namespace MyProject.Application.Common.Exceptions;

public abstract class ApplicationExceptionBase : Exception
{
    public string Code { get; }
    public ErrorType Type { get; }

    protected ApplicationExceptionBase(
        string code,
        string message,
        ErrorType type,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Type = type;
    }
}
