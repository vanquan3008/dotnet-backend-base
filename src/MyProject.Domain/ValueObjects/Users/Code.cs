using MyProject.Domain.Errors.Users;
using MyProject.Domain.Exceptions;

namespace MyProject.Domain.ValueObjects.Users;

public sealed record Code
{
    public const int MaxLength = 50;
    public string Value { get; }

    private Code(string value) => Value = value;

    public static Code Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(UserErrorCodes.CodeRequired, "Code is required.");
        }

        value = value.Trim();
        if (value.Length > MaxLength)
        {
            throw new DomainException(UserErrorCodes.CodeTooLong, $"Code must not exceed {MaxLength} characters.");
        }

        if (!value.All(c => char.IsLetterOrDigit(c) || c is '_' or '-'))
        {
            throw new DomainException(
                UserErrorCodes.CodeInvalidFormat,
                "Code may contain only letters, numbers, underscores, and hyphens.");
        }

        return new Code(value.ToUpperInvariant());
    }

    public override string ToString() => Value;
}
